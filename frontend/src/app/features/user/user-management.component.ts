import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthSession, CreateUserRequest, UserSummary } from './models/user.models';
import { UserApiService } from './user-api.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateUserDialogComponent } from './create-user-dialog.component';
import { MatCardModule } from "@angular/material/card";
import { MatIconModule } from "@angular/material/icon";
import { AppTableComponent } from "../../shared/components/app-table/app-table.component";
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatIconModule, AppTableComponent, MatTableModule],
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  public createForm: CreateUserRequest = {
    email: '',
    fullName: '',
    password: ''
  };

  displayedColumns = [
    'user',
    'email',
    'status',
    'created',
    'actions'
  ];

  public users: UserSummary[] = [];
  public session: AuthSession | null = null;
  public loading = false;
  public error = '';
  public notice = '';

  private dialog = inject(MatDialog);
  private cd = inject(ChangeDetectorRef);
  constructor(private readonly userApi: UserApiService) {}

  async ngOnInit(): Promise<void> {
    await this.loadUsers();
  }

  protected get totalUsers(): number {
    return this.users.length;
  }

  protected get activeUsers(): number {
    return this.users.filter((user) => user.isActive).length;
  }

  async openCreateUserDialog(): Promise<void> {
    const dialogRef = this.dialog.open(CreateUserDialogComponent, {
      width: '400px'
    });

    const result = await dialogRef.afterClosed().toPromise();
    if (result) {
      this.error = '';
      this.notice = '';
      this.loading = true;
      try {
        await this.userApi.createUser(result);
        await this.loadUsers();
        this.notice = 'User created successfully.';
      } catch (error) {
        this.error = this.extractMessage(error, 'Could not create the user.');
      } finally {
        this.loading = false;
      }
    }
  }

  async deleteUser(user: UserSummary): Promise<void> {
    const approved = window.confirm(`Delete ${user.fullName}? This cannot be undone.`);
    if (!approved) {
      return;
    }

    this.error = '';
    this.notice = '';
    this.loading = true;

    try {
      if (this.session?.user.id !== user.id) {
        await this.userApi.deleteUser(user.id);
        this.notice = 'User deleted successfully.';
      }

      await this.loadUsers();
    } catch (error) {
      this.error = this.extractMessage(error, 'Could not delete the user.');
    } finally {
      this.loading = false;
    }
  }

  protected trackByUserId(_: number, user: UserSummary): string {
    return user.id;
  }

  protected formatDate(value: string): string {
    return new Date(value).toLocaleString(undefined, {
      dateStyle: 'medium',
      timeStyle: 'short'
    });
  }

  private async loadUsers(): Promise<void> {
    try {
      const response = await this.userApi.listUsers();
      this.users = response;
      this.cd.detectChanges();
    } catch (error) {
      this.error = this.extractMessage(error, 'Could not load users.');
    }
  }

  private extractMessage(error: unknown, fallback: string): string {
    if (error instanceof Error && error.message.trim()) {
      return error.message;
    }

    return fallback;
  }
}
