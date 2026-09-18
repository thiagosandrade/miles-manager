import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { TranslatePipe } from '@ngx-translate/core';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { UserApiService } from './user-api.service';
import { UserDetails, CreateUserRequest, UpdateUserRequest } from './models/user.models';
import { CreateUserDialogComponent } from './create-user-dialog.component';
import { SnackbarService } from '../../core/services/snackbar.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatIconModule, MatButtonModule, MatDialogModule, MatSnackBarModule, AppTableComponent, TranslatePipe, MatInputModule, MatFormFieldModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  private api = inject(UserApiService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  displayedColumns = ['user', 'email', 'status', 'created', 'actions'];
  readonly allData = signal<UserDetails[]>([]);
  readonly data = signal<UserDetails[]>([]);
  readonly loading = signal(false);
  readonly isRateLimitReached = signal(false);
  pagedData: UserDetails[] = [];

  private pageIndex = 0;
  private pageSize = 5;
  private currentFilter = '';
  private sortActive = '';
  private sortDirection: 'asc' | 'desc' | '' = '';

  ngOnInit(): void {
    this.loadUsers();
  }

  get totalUsers(): number { return this.data().length; }
  get activeUsers(): number { return this.data().filter(user => user.isActive).length; }

  loadUsers(): void {
    this.loading.set(true);
    this.isRateLimitReached.set(false);

    this.api.listUsers().pipe(
      catchError(() => {
        this.loading.set(false);
        this.isRateLimitReached.set(true);
        this.snackbarService.error('Could not load users', 3000);
        return of(null);
      })
    ).subscribe(users => {
      if (!users) return;
      this.allData.set(users);
      this.applyFilter(this.currentFilter);
      this.loading.set(false);
    });
  }

  onSortChange(event: Sort): void {
    this.sortActive = event.direction ? event.active : '';
    this.sortDirection = event.direction;
    this.pageIndex = 0;
    this.applyFilter(this.currentFilter);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  applyFilter(filter: string): void {
    this.currentFilter = filter;
    const normalizedFilter = filter.trim().toLowerCase();
    const filtered = this.allData().filter(item =>
      [item.fullName, item.email].some(value => value?.toLowerCase().includes(normalizedFilter))
    );

    this.data.set(this.sortData(filtered));
    this.updatePagedData();
  }

  private sortData(values: UserDetails[]): UserDetails[] {
    if (!this.sortActive || !this.sortDirection) return values;

    return [...values].sort((left, right) => {
      const a = this.getSortValue(left);
      const b = this.getSortValue(right);
      const result = typeof a === 'string' && typeof b === 'string'
        ? a.localeCompare(b, undefined, { sensitivity: 'base' })
        : a < b ? -1 : a > b ? 1 : 0;
      return this.sortDirection === 'asc' ? result : -result;
    });
  }

  private getSortValue(user: UserDetails): string | number {
    switch (this.sortActive) {
      case 'user': return user.fullName ?? '';
      case 'email': return user.email ?? '';
      case 'status': return user.isActive ? 1 : 0;
      case 'created': return new Date(user.createdAtUtc).getTime();
      default: return '';
    }
  }

  private updatePagedData(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedData = this.data().slice(start, start + this.pageSize);
  }

  openCreateUserDialog(): void {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, { width: '450px', maxWidth: 'calc(100vw - 32px)', data: { operation: 'create' } });
    dialogRef.afterClosed().subscribe((result: CreateUserRequest | undefined) => {
      if (!result) return;
      this.api.createUser(result).subscribe({
        next: () => { this.snackbarService.success('User created succesfully.', 3000); this.loadUsers(); },
        error: ex => { this.snackbarService.error(`Could not create user. ${ex.error.title}`, 3000); }
      });
    });
  }

  openEditUserDialog(user: UserDetails): void {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, { width: '450px', maxWidth: 'calc(100vw - 32px)', data: { operation: 'update', user } });
    dialogRef.afterClosed().subscribe((result: UpdateUserRequest | undefined) => {
      if (!result) return;
      this.api.updateUser(result).subscribe({
        next: () => { this.snackbarService.success('User updated succesfully.', 3000); this.loadUsers(); },
        error: ex => { this.snackbarService.error(`Could not update user. ${ex.error.title}`, 3000); }
      });
    });
  }

  deleteUser(user: UserDetails): void {
    this.api.deleteUser(user.id).subscribe({
      next: () => { this.snackbarService.success('User deleted succesfully.', 3000); this.loadUsers(); },
      error: ex => { this.snackbarService.error(`Could not delete user. ${ex.error.title}`, 3000); }
    });
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });
  }
}
