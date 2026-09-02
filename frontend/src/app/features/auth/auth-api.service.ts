import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from './models/authentication.model';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { Router } from '@angular/router';

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
