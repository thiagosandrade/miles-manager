import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';

import { UserApiService } from '../user/user-api.service';
import { UserDetails } from '../user/models/user.models';

import { EditUserRolesDialogComponent } from './edit-user-roles-dialog.component';

import { TranslatePipe } from '@ngx-translate/core';
import { AppTableComponent } from "../../shared/components/app-table/app-table.component";
import { MatInputModule } from '@angular/material/input';

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
    MatInputModule,
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

  readonly allData = signal<UserDetails[]>([]);
  readonly data = signal<UserDetails[]>([]);

  readonly pagedData = signal<UserDetails[]>([]);
  private pageIndex = 0;
  private pageSize = 5;

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

        this.allData.set(users);
        this.data.set(users);
        this.updatePagedData();

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

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  private updatePagedData(): void {
    const values = this.data();
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(values.slice(start, start + this.pageSize));
  }

    applyFilter(filter: string): void {

    const normalizedFilter = filter.trim().toLowerCase();

    const filtered = this.allData().filter(item =>
      item.fullName?.toLowerCase().includes(normalizedFilter) ?? false
    );

    this.data.set(filtered);
    this.updatePagedData();

  }

  openUpdateDialog(user: UserDetails): void {

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
