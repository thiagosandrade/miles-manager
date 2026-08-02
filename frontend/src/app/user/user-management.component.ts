import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthSession, CreateUserRequest, UserSummary } from './models/user.models';
import { UserApiService } from './user-api.service';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  protected readonly loginForm = {
    email: 'admin@milesmanager.local',
    password: 'Password123!'
  };

  protected createForm: CreateUserRequest = {
    email: '',
    fullName: '',
    password: ''
  };

  protected users: UserSummary[] = [];
  protected session: AuthSession | null = null;
  protected loading = false;
  protected error = '';
  protected notice = '';
  protected connectionMode: 'api' | 'local' = 'local';
  protected lastSyncedAt = '';

  constructor(private readonly userApi: UserApiService) {}

  async ngOnInit(): Promise<void> {
    this.session = this.userApi.getSession();
    await this.loadUsers();
  }

  protected get totalUsers(): number {
    return this.users.length;
  }

  protected get activeUsers(): number {
    return this.users.filter((user) => user.isActive).length;
  }

  protected get localModeLabel(): string {
    return this.connectionMode === 'api' ? 'API connected' : 'Local demo mode';
  }

  async login(): Promise<void> {
    this.error = '';
    this.notice = '';
    this.loading = true;

    try {
      const response = await this.userApi.login(this.loginForm);
      this.session = response.session;
      this.connectionMode = response.mode;
      this.notice = response.mode === 'api' ? 'Signed in against the backend.' : 'Signed in using the local demo store.';
      await this.loadUsers();
    } catch (error) {
      this.error = this.extractMessage(error, 'Login failed.');
    } finally {
      this.loading = false;
    }
  }

  async logout(): Promise<void> {
    if (!this.session) {
      this.clearLocalSession();
      return;
    }

    this.error = '';
    this.notice = '';
    this.loading = true;

    try {
      const response = await this.userApi.logout(this.session.user.id);
      this.connectionMode = response.mode;
    } catch {
      // Sign out should still clear the local session.
    } finally {
      this.clearLocalSession();
      this.loading = false;
      this.notice = 'You have been signed out.';
    }
  }

  async createUser(): Promise<void> {
    this.error = '';
    this.notice = '';
    this.loading = true;

    try {
      await this.userApi.createUser(this.createForm);
      await this.loadUsers();
      this.createForm = { email: '', fullName: '', password: '' };
      this.notice = 'User created successfully.';
    } catch (error) {
      this.error = this.extractMessage(error, 'Could not create the user.');
    } finally {
      this.loading = false;
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
      await this.userApi.deleteUser(user.id);
      if (this.session?.user.id === user.id) {
        await this.logout();
        return;
      }

      await this.loadUsers();
      this.notice = 'User deleted successfully.';
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
      this.users = response.users;
      this.connectionMode = response.mode;
      this.lastSyncedAt = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    } catch (error) {
      this.error = this.extractMessage(error, 'Could not load users.');
    }
  }

  private clearLocalSession(): void {
    this.userApi.clearSession();
    this.session = null;
  }

  private extractMessage(error: unknown, fallback: string): string {
    if (error instanceof Error && error.message.trim()) {
      return error.message;
    }

    return fallback;
  }
}
