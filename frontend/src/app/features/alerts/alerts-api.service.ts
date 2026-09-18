import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Alert } from './models/alert.model';
import { AlertSource, AlertSourceCreateRequest, AlertSourceUpdateRequest } from './models/alert-source.model';

@Injectable({ providedIn: 'root' })
export class AlertsApiService {
  private readonly http = inject(HttpClient);
  private readonly alertsUrl = `${environment.apiBaseUrl}/alerts`;
  private readonly alertSourcesUrl = `${environment.apiBaseUrl}/alert-sources`;

  getAlerts(): Observable<Alert[]> {
    return this.http.get<Alert[]>(this.alertsUrl);
  }

  deleteAlert(id: string): Observable<void> {
    return this.http.delete<void>(`${this.alertsUrl}/${id}`);
  }

  getAlertSources(): Observable<AlertSource[]> {
    return this.http.get<AlertSource[]>(this.alertSourcesUrl);
  }

  createAlertSource(request: AlertSourceCreateRequest): Observable<string> {
    return this.http.post<string>(this.alertSourcesUrl, request);
  }

  updateAlertSource(id: string, request: AlertSourceUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.alertSourcesUrl}/${id}`, { id, ...request });
  }

  deleteAlertSource(id: string): Observable<void> {
    return this.http.delete<void>(`${this.alertSourcesUrl}/${id}`);
  }
}
