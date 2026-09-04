import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse } from './models/authentication.model';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { Router } from '@angular/router';

export interface RegisterRequest {
  email: string;
  fullName: string;
  password: string;
}

export interface RegisterResponse {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
}

export interface ForgotPasswordResponse {
  message?: string;
}

export interface PasswordResetTokenValidation {
  valid: boolean;
  email: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthApiService {

  private readonly tokenStorageService = inject(TokenStorageService);
  private readonly apiBaseUrl = `${environment.apiBaseUrl}/auth`;

  constructor(private http: HttpClient, private router: Router) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiBaseUrl}/login`, request);
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiBaseUrl}/register`, request);
  }

  forgotPassword(email: string): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/forgot-password`, { email });
  }

  validatePasswordResetToken(token: string): Observable<PasswordResetTokenValidation> {
    return this.http.get<PasswordResetTokenValidation>(
      `${this.apiBaseUrl}/reset-password/validate`,
      { params: { token } }
    );
  }

  resetPassword(token: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiBaseUrl}/reset-password`, {
      token,
      newPassword
    });
  }

  refresh(): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiBaseUrl}/refresh`, {});
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.apiBaseUrl}/logout`, {})
      .pipe(
        finalize(() => this.logoutLocal())
      );
  }

  logoutLocal(): void {
    this.tokenStorageService.logoutToken();
    this.router.navigate(['/auth']);
  }
}
