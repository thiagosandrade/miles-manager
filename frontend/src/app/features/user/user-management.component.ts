import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

import { UserApiService } from './user-api.service';
import { UserSummary, CreateUserRequest } from './models/user.models';
import { CreateUserDialogComponent } from './create-user-dialog.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule,
    AppTableComponent,
    TranslatePipe
  ],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  private api = inject(UserApiService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private cd = inject(ChangeDetectorRef);

  displayedColumns = [
    'user',
    'email',
    'status',
    'created',
    'actions'
  ];

  data: UserSummary[] = [];

  isLoadingResults = true;
  isRateLimitReached = false;

  ngOnInit(): void {
    this.loadUsers();
  }

  get totalUsers(): number {
    return this.data.length;
  }

  get activeUsers(): number {
    return this.data.filter(x => x.isActive).length;
  }

  loadUsers(): void {

    this.isLoadingResults = true;

    this.api
      .listUsers()
      .pipe(
        catchError(() => {

          this.isLoadingResults = false;
          this.isRateLimitReached = true;

          this.snackbarService.error('Could not load users', 3000);


          return of([]);

        })
      )
      .subscribe(users => {

        this.data = users;

        this.isLoadingResults = false;
        this.isRateLimitReached = false;

        this.cd.detectChanges();

      });

  }

  openCreateUserDialog(): void {

    const dialogRef = this.dialog.open(CreateUserDialogComponent, {
      width: '450px'
    });

    dialogRef.afterClosed().subscribe((result: CreateUserRequest | undefined) => {

      if (!result) {
        return;
      }

      this.api.createUser(result).subscribe({

        next: () => {
          this.snackbarService.error('User created succesfully.', 3000);
          this.loadUsers();
        },

        error: () => {
          this.snackbarService.error('Could not create user.', 3000);
        }

      });

    });

  }

  deleteUser(user: UserSummary): void {

    this.api.deleteUser(user.id).subscribe({

      next: () => {
        this.snackbarService.error('User deleted succesfully.', 3000);
        this.loadUsers();
      },

      error: () => {
        this.snackbarService.error('Could not delete user.', 3000);
      }
    });

  }

  formatDate(value: string): string {

    return new Date(value).toLocaleString(undefined, {

      dateStyle: 'medium',
      timeStyle: 'short'

    });

  }

}