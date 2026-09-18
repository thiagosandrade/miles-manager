import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

interface ApiOptions {
  headers?: HttpHeaders;
  params?: HttpParams;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);

  get<T>(endpoint: string, options?: ApiOptions): Observable<T> {
    return this.http.get<T>(endpoint, options);
  }

  post<T>(endpoint: string, body: any, options?: ApiOptions): Observable<T> {
    return this.http.post<T>(endpoint, body, options);
  }

  put<T>(endpoint: string, body: any, options?: ApiOptions): Observable<T> {
    return this.http.put<T>(endpoint, body, options);
  }

  delete<T>(endpoint: string, options?: ApiOptions): Observable<T> {
    return this.http.delete<T>(endpoint, options);
  }
}
