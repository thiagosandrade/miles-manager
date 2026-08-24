import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateUserRequest, UpdateUserRequest, UserDetails } from './models/user.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {

  private apiBaseUrl = `${environment.apiBaseUrl}/users`;

  constructor(private readonly http: HttpClient) {}

  listUsers(): Observable<UserDetails[]> {
    return this.http.get<UserDetails[]>(`${this.apiBaseUrl}`);
  }

  getUser(id: string): Observable<UserDetails> {
    return this.http.get<UserDetails>(`${this.apiBaseUrl}/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<UserDetails> {
    return this.http.post<UserDetails>(`${this.apiBaseUrl}`, request);
  }

  updateUser(request: UpdateUserRequest): Observable<UserDetails> {
    return this.http.put<UserDetails>(`${this.apiBaseUrl}/${request.id}`, request);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
