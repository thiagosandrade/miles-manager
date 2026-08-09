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

  async listUsers(): Promise<UserSummary[]> {
    try {
      return await firstValueFrom(this.http.get<UserSummary[]>(`${this.apiBaseUrl}`));
    } catch (error) {
      throw error;
    }
  }

  async getUser(id: string): Promise<UserDetails> {
    try {
      return await firstValueFrom(this.http.get<UserDetails>(`${this.apiBaseUrl}/${id}`));
    } catch (error) {
      throw error;
    }
  }

  async createUser(request: CreateUserRequest): Promise<UserDetails> {
    try {
      return await firstValueFrom(this.http.post<UserDetails>(`${this.apiBaseUrl}`, request));
    } catch (error) {
      throw error;
    }
  }

  async deleteUser(id: string): Promise<void> {
    try {
      await firstValueFrom(this.http.delete<void>(`${this.apiBaseUrl}/${id}`));
    } catch (error) {
      throw error;
    }
  }
}
