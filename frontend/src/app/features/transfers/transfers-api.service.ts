import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Transfer, TransferCreateRequest, TransferListResponse, TransferUpdateRequest } from './models/mileage-transfer.model';

@Injectable({
  providedIn: 'root'
})
export class TransfersApiService {

  private apiBaseUrl = `${environment.apiBaseUrl}/mileage-transfers`;

  constructor(private http: HttpClient) {}

  listTransfers(params?: { page?: number; pageSize?: number; sort?: string; filter?: string }): Observable<TransferListResponse> {
    let httpParams = new HttpParams();
    if (params?.page !== undefined) httpParams = httpParams.set('page', params.page.toString());
    if (params?.pageSize !== undefined) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.sort) httpParams = httpParams.set('sort', params.sort);
    if (params?.filter) httpParams = httpParams.set('filter', params.filter);
    return this.http.get<TransferListResponse>(`${this.apiBaseUrl}`, { params: httpParams });
  }

  getTransfer(id: string): Observable<Transfer> {
    return this.http.get<Transfer>(`${this.apiBaseUrl}/${id}`);
  }

  createTransfer(request: TransferCreateRequest): Observable<Transfer> {
    return this.http.post<Transfer>(`${this.apiBaseUrl}`, request);
  }

  updateTransfer(id: string, request: TransferUpdateRequest): Observable<Transfer> {
    return this.http.put<Transfer>(`${this.apiBaseUrl}/${id}`, request);
  }

  deleteTransfer(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
