import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AuthSession, CreateUserRequest, LoginRequest, UserDetails, UserSummary } from './models/user.models';

interface LocalUserRecord extends UserDetails {
  password: string;
  tokenVersion: number;
}

interface OperationResult<T> {
  status: number;
  value?: T | null;
}

@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  private readonly apiBaseUrl = 'https://localhost:49922/api';
  private readonly sessionKey = 'miles-manager.session';
  private readonly usersKey = 'miles-manager.local-users';

  constructor(private readonly http: HttpClient) {}

  async login(request: LoginRequest): Promise<{ session: AuthSession; mode: 'api' | 'local' }> {
    try {
      const response = await firstValueFrom(this.http.post<AuthSession | OperationResult<AuthSession>>(`${this.apiBaseUrl}/auth/login`, request));
      const session = this.extractAuthSession(response);
      if (session) {
        this.saveSession(session);
        return { session, mode: 'api' };
      }

      throw new Error('Invalid email or password.');
    } catch (error) {
      if (this.shouldFallback(error)) {
        const session = this.loginLocal(request);
        this.saveSession(session);
        return { session, mode: 'local' };
      }

      throw this.normalizeError(error, 'Invalid email or password.');
    }
  }

  async logout(userId: string): Promise<{ mode: 'api' | 'local' }> {
    const session = this.getSession();

    try {
      await firstValueFrom(this.http.post<unknown>(`${this.apiBaseUrl}/auth/logout`, { userId }));
      this.clearSession();
      return { mode: 'api' };
    } catch (error) {
      if (!this.shouldFallback(error)) {
        throw this.normalizeError(error, 'Could not sign out.');
      }

      this.clearSession();
      if (session?.user?.id) {
        this.touchLocalLogout(session.user.id);
      }
      return { mode: 'local' };
    }
  }

  async listUsers(): Promise<{ users: UserSummary[]; mode: 'api' | 'local' }> {
    try {
      const response = await firstValueFrom(this.http.get<UserSummary[] | OperationResult<UserSummary[]> | { items: UserSummary[] }>(`${this.apiBaseUrl}/users`));
      const users = this.extractUsers(response);
      return { users, mode: 'api' };
    } catch (error) {
      if (!this.shouldFallback(error)) {
        throw this.normalizeError(error, 'Could not load users.');
      }

      return { users: this.getLocalUsers().map((user) => this.toSummary(user)), mode: 'local' };
    }
  }

  async createUser(request: CreateUserRequest): Promise<{ user: UserDetails; mode: 'api' | 'local' }> {
    try {
      const response = await firstValueFrom(this.http.post<UserDetails | OperationResult<UserDetails>>(`${this.apiBaseUrl}/users`, request));
      const user = this.extractUserDetails(response);
      if (user) {
        return { user, mode: 'api' };
      }

      throw new Error('A user with this email already exists.');
    } catch (error) {
      if (!this.shouldFallback(error)) {
        throw this.normalizeError(error, 'Could not create the user.');
      }

      return { user: this.createLocalUser(request), mode: 'local' };
    }
  }

  async deleteUser(id: string): Promise<{ mode: 'api' | 'local' }> {
    try {
      await firstValueFrom(this.http.delete<unknown>(`${this.apiBaseUrl}/users/${id}`));
      return { mode: 'api' };
    } catch (error) {
      if (!this.shouldFallback(error)) {
        throw this.normalizeError(error, 'Could not delete the user.');
      }

      this.deleteLocalUser(id);
      return { mode: 'local' };
    }
  }

  getSession(): AuthSession | null {
    const raw = localStorage.getItem(this.sessionKey);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthSession;
    } catch {
      return null;
    }
  }

  clearSession(): void {
    localStorage.removeItem(this.sessionKey);
  }

  private shouldFallback(error: unknown): boolean {
    return error instanceof HttpErrorResponse && (error.status === 0 || error.status === 404);
  }

  private normalizeError(error: unknown, fallback: string): Error {
    if (error instanceof Error && error.message.trim()) {
      return error;
    }

    return new Error(fallback);
  }

  private extractAuthSession(response: AuthSession | OperationResult<AuthSession>): AuthSession | null {
    if (this.hasOperationStatus(response)) {
      if (response.status !== 1 || response.value == null) {
        return null;
      }

      return response.value;
    }

    return response;
  }

  private extractUsers(response: UserSummary[] | OperationResult<UserSummary[]> | { items: UserSummary[] }): UserSummary[] {
    if (Array.isArray(response)) {
      return response;
    }

    if (this.hasOperationStatus(response)) {
      if (response.status !== 1 || response.value == null) {
        return [];
      }

      return response.value;
    }

    if (this.hasItemsArray(response)) {
      return response.items;
    }

    return [];
  }

  private extractUserDetails(response: UserDetails | OperationResult<UserDetails>): UserDetails | null {
    if (this.hasOperationStatus(response)) {
      if (response.status !== 1 || response.value == null) {
        return null;
      }

      return response.value;
    }

    return response;
  }

  private hasOperationStatus<T>(value: T | OperationResult<T> | unknown): value is OperationResult<T> {
    return !!value && typeof value === 'object' && 'status' in value;
  }

  private hasItemsArray(value: unknown): value is { items: UserSummary[] } {
    return !!value && typeof value === 'object' && 'items' in value && Array.isArray((value as { items?: UserSummary[] }).items);
  }

  private saveSession(session: AuthSession): void {
    localStorage.setItem(this.sessionKey, JSON.stringify(session));
  }

  private loginLocal(request: LoginRequest): AuthSession {
    const users = this.getLocalUsers();
    const normalizedEmail = request.email.trim().toLowerCase();
    const user = users.find((entry) => entry.email === normalizedEmail && entry.password === request.password && entry.isActive);

    if (!user) {
      throw new Error('Invalid email or password.');
    }

    user.lastLoginAtUtc = new Date().toISOString();
    user.updatedAtUtc = new Date().toISOString();
    user.tokenVersion += 1;
    this.storeLocalUsers(users);

    return {
      token: {
        accessToken: `local-${crypto.randomUUID()}`,
        expiresAtUtc: new Date(Date.now() + 1000 * 60 * 60 * 8).toISOString()
      },
      user: this.toSummary(user)
    };
  }

  private createLocalUser(request: CreateUserRequest): UserDetails {
    const users = this.getLocalUsers();
    const normalizedEmail = request.email.trim().toLowerCase();

    if (users.some((entry) => entry.email === normalizedEmail)) {
      throw new Error('A user with this email already exists.');
    }

    const now = new Date().toISOString();
    const user: LocalUserRecord = {
      id: crypto.randomUUID(),
      email: normalizedEmail,
      fullName: request.fullName.trim(),
      isActive: true,
      createdAtUtc: now,
      updatedAtUtc: now,
      lastLoginAtUtc: null,
      password: request.password,
      tokenVersion: 0
    };

    users.unshift(user);
    this.storeLocalUsers(users);
    return this.toDetails(user);
  }

  private deleteLocalUser(id: string): void {
    const users = this.getLocalUsers().filter((user) => user.id !== id);
    this.storeLocalUsers(users);
  }

  private touchLocalLogout(userId: string): void {
    const users = this.getLocalUsers();
    const user = users.find((entry) => entry.id === userId);

    if (!user) {
      return;
    }

    user.tokenVersion += 1;
    user.updatedAtUtc = new Date().toISOString();
    this.storeLocalUsers(users);
  }

  private getLocalUsers(): LocalUserRecord[] {
    const raw = localStorage.getItem(this.usersKey);
    if (raw) {
      try {
        return JSON.parse(raw) as LocalUserRecord[];
      } catch {
        // Fall through to seeding below.
      }
    }

    const seeded: LocalUserRecord[] = [
      {
        id: crypto.randomUUID(),
        email: 'admin@milesmanager.local',
        fullName: 'Miles Manager Admin',
        isActive: true,
        createdAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 24 * 8).toISOString(),
        updatedAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 12).toISOString(),
        lastLoginAtUtc: null,
        password: 'Password123!',
        tokenVersion: 0
      },
      {
        id: crypto.randomUUID(),
        email: 'operations@milesmanager.local',
        fullName: 'Operations Lead',
        isActive: true,
        createdAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 24 * 4).toISOString(),
        updatedAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 5).toISOString(),
        lastLoginAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 2).toISOString(),
        password: 'Password123!',
        tokenVersion: 0
      },
      {
        id: crypto.randomUUID(),
        email: 'disabled@milesmanager.local',
        fullName: 'Disabled Example',
        isActive: false,
        createdAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 24 * 14).toISOString(),
        updatedAtUtc: new Date(Date.now() - 1000 * 60 * 60 * 24 * 2).toISOString(),
        lastLoginAtUtc: null,
        password: 'Password123!',
        tokenVersion: 0
      }
    ];

    this.storeLocalUsers(seeded);
    return seeded;
  }

  private storeLocalUsers(users: LocalUserRecord[]): void {
    localStorage.setItem(this.usersKey, JSON.stringify(users));
  }

  private toSummary(user: LocalUserRecord): UserSummary {
    return {
      id: user.id,
      email: user.email,
      fullName: user.fullName,
      isActive: user.isActive,
      createdAtUtc: user.createdAtUtc
    };
  }

  private toDetails(user: LocalUserRecord): UserDetails {
    return {
      ...this.toSummary(user),
      updatedAtUtc: user.updatedAtUtc,
      lastLoginAtUtc: user.lastLoginAtUtc
    };
  }
}
