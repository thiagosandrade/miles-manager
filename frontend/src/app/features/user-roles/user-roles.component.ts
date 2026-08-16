import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';

import { UserApiService } from '../user/user-api.service';
import { UserSummary } from '../user/models/user.models';

import { EditUserRolesDialogComponent } from './edit-user-roles-dialog.component';

import { TranslatePipe } from '@ngx-translate/core';
import { AppTableComponent } from "../../shared/components/app-table/app-table.component";

@Component({
  selector: 'app-user-roles',
  standalone: true,
  templateUrl: './user-roles.component.html',
  styleUrls: ['./user-roles.component.scss'],
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    TranslatePipe,
    AppTableComponent
]
})
export class UserRolesComponent implements OnInit {

  private readonly userApi = inject(UserApiService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(true);

  readonly data = signal<UserSummary[]>([]);

  readonly displayedColumns = [
    'fullName',
    'roles',
    'email',
    'actions'
  ];

  isRateLimitReached = false;

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {

    this.loading.set(true);
    this.isRateLimitReached = false;

    this.userApi.listUsers().subscribe({

      next: users => {

        this.data.set(users);

        this.loading.set(false);

      },

      error: () => {

        this.loading.set(false);

        this.isRateLimitReached = true;

        this.snackBar.open(
          'Could not load users.',
          'Close',
          {
            duration: 3000
          });

      }

    });

  }

  openUpdateDialog(user: UserSummary): void {

    const dialogRef = this.dialog.open(
      EditUserRolesDialogComponent,
      {
        width: '700px',
        data: {
          userId: user.id
        }
      });

    dialogRef.afterClosed().subscribe(updated => {

      if (updated) {

        this.loadData();

      }

    });

  }

}