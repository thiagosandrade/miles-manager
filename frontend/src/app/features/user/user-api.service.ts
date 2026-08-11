import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { CreateUserRequest, UserDetails, UserSummary } from './models/user.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {

  private apiBaseUrl = `${environment.apiBaseUrl}/users`;

  constructor(private readonly http: HttpClient) {}

  listUsers(): Observable<UserSummary[]> {
    return this.http.get<UserSummary[]>(`${this.apiBaseUrl}`);
  }

  getUser(id: string): Observable<UserDetails> {
    return this.http.get<UserDetails>(`${this.apiBaseUrl}/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<UserDetails> {
    return this.http.post<UserDetails>(`${this.apiBaseUrl}`, request);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
