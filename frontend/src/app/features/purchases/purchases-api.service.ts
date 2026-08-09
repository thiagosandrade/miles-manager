import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MileagePurchase, PurchaseCreateRequest, PurchaseUpdateRequest } from './models/mileage-purchase.model';

@Injectable({
  providedIn: 'root'
})
export class PurchasesApiService {

  private apiBaseUrl = `${environment.apiBaseUrl}/mileage-purchases`;

  constructor(private http: HttpClient) {}

  listPurchases(params?: { page?: number; pageSize?: number; sort?: string; filter?: string }): Observable<MileagePurchase[]> {
    let httpParams = new HttpParams();
    if (params?.page !== undefined) httpParams = httpParams.set('page', params.page.toString());
    if (params?.pageSize !== undefined) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.sort) httpParams = httpParams.set('sort', params.sort);
    if (params?.filter) httpParams = httpParams.set('filter', params.filter);
    return this.http.get<MileagePurchase[]>(`${this.apiBaseUrl}`, { params: httpParams });
  }

  getPurchase(id: string): Observable<MileagePurchase> {
    return this.http.get<MileagePurchase>(`${this.apiBaseUrl}/${id}`);
  }

  createPurchase(request: PurchaseCreateRequest): Observable<MileagePurchase> {
    return this.http.post<MileagePurchase>(`${this.apiBaseUrl}`, request);
  }

  updatePurchase(id: string, request: PurchaseUpdateRequest): Observable<MileagePurchase> {
    return this.http.put<MileagePurchase>(`${this.apiBaseUrl}/${id}`, request);
  }

  deletePurchase(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
