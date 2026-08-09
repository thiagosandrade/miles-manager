import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { DashboardDto } from './models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardApiService {

  private readonly apiBaseUrl = `${environment.apiBaseUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  async getDashboard(): Promise<DashboardDto> {
    return await firstValueFrom(this.http.get<DashboardDto>(`${this.apiBaseUrl}`));
  }

}
