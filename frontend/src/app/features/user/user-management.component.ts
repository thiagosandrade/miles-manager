import { ChangeDetectorRef, Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { PageEvent } from '@angular/material/paginator';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { UserApiService } from './user-api.service';
import { UserDetails, CreateUserRequest, UpdateUserRequest } from './models/user.models';
import { CreateUserDialogComponent } from './create-user-dialog.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-user-management', standalone: true,
  imports: [CommonModule, MatTableModule, MatIconModule, MatButtonModule, MatDialogModule, MatSnackBarModule, AppTableComponent, TranslatePipe, MatInputModule],
  templateUrl: './user-management.component.html', styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent implements OnInit {
  private api = inject(UserApiService); private dialog = inject(MatDialog); private snackbarService = inject(SnackbarService); private cd = inject(ChangeDetectorRef);
  displayedColumns = ['user','email','status','created','actions'];
  readonly allData = signal<UserDetails[]>([]); readonly data = signal<UserDetails[]>([]); readonly loading = signal<boolean>(false);
  pagedData: UserDetails[] = []; private pageIndex = 0; private pageSize = 5; private currentFilter = '';
  isRateLimitReached = signal(false);
  ngOnInit(): void { this.loadUsers(); }
  get totalUsers(): number { return this.data.length; }
  get activeUsers(): number { return this.data().filter(x => x.isActive).length; }
  loadUsers(): void {
    this.loading.set(true); this.isRateLimitReached.set(false);
    this.api.listUsers().pipe(catchError(() => { this.loading.set(false); this.isRateLimitReached.set(true); this.snackbarService.error('Could not load users', 3000); return of(null); })).subscribe(users => {
      if (users) { this.allData.set(users); this.applyFilter(this.currentFilter); this.loading.set(false); this.isRateLimitReached.set(false); }
    });
  }
  onPageChange(event: PageEvent): void { this.pageIndex = event.pageIndex; this.pageSize = event.pageSize; this.updatePagedData(); }
  private updatePagedData(): void { const start = this.pageIndex * this.pageSize; this.pagedData = this.data().slice(start, start + this.pageSize); }
  applyFilter(filter: string): void {
    this.currentFilter = filter;
    const normalizedFilter = filter.trim().toLowerCase();
    const filtered = this.allData().filter(item => item.fullName?.toLowerCase().includes(normalizedFilter) ?? false);
    this.data.set(filtered); this.updatePagedData();
  }
  openCreateUserDialog(): void {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, { width: '450px', maxWidth: 'calc(100vw - 32px)', data: { operation: 'create' } });
    dialogRef.afterClosed().subscribe((result: CreateUserRequest | undefined) => {
      if (!result) return;
      this.api.createUser(result).subscribe({ next: () => { this.snackbarService.success('User created succesfully.', 3000); this.loadUsers(); }, error: (ex) => { this.snackbarService.error(`Could not create user. ${ex.error.title}`, 3000); } });
    });
  }
  openEditUserDialog(user: UserDetails): void {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, { width: '450px', maxWidth: 'calc(100vw - 32px)', data: { operation: 'update', user } });
    dialogRef.afterClosed().subscribe((result: UpdateUserRequest | undefined) => {
      if (!result) return;
      this.api.updateUser(result).subscribe({ next: () => { this.snackbarService.success('User updated succesfully.', 3000); this.loadUsers(); }, error: (ex) => { this.snackbarService.error(`Could not update user. ${ex.error.title}`, 3000); } });
    });
  }
  deleteUser(user: UserDetails): void {
    this.api.deleteUser(user.id).subscribe({ next: () => { this.snackbarService.success('User deleted succesfully.', 3000); this.loadUsers(); }, error: (ex) => { this.snackbarService.error(`Could not delete user. ${ex.error.title}`, 3000); } });
  }
  formatDate(value: string): string { return new Date(value).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' }); }
}
