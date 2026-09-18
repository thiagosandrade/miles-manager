import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MileageTransfer, TransferCreateRequest, TransferUpdateRequest } from './models/mileage-transfer.model';

@Injectable({ providedIn: 'root' })
export class TransfersApiService {
  private apiBaseUrl = `${environment.apiBaseUrl}/mileage-transfers`;

  constructor(private http: HttpClient) {}

  listTransfers(): Observable<MileageTransfer[]> {
    const params = new HttpParams()
      .set('page', '1')
      .set('pageSize', '10000');

    return this.http.get<MileageTransfer[]>(this.apiBaseUrl, { params });
  }

  getTransfer(id: string): Observable<MileageTransfer> {
    return this.http.get<MileageTransfer>(`${this.apiBaseUrl}/${id}`);
  }

  createTransfer(request: TransferCreateRequest): Observable<MileageTransfer> {
    return this.http.post<MileageTransfer>(this.apiBaseUrl, request);
  }

  updateTransfer(id: string, request: TransferUpdateRequest): Observable<MileageTransfer> {
    return this.http.put<MileageTransfer>(`${this.apiBaseUrl}/${id}`, request);
  }

  deleteTransfer(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
