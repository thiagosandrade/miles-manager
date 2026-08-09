import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);

  get<T>(endpoint: string, options?: {headers?: HttpHeaders}): Observable<T> {
    return this.http.get<T>(`${endpoint}`, options);
  }

  post<T>(endpoint: string, body: any, options?: {headers?: HttpHeaders}): Observable<T> {
    return this.http.post<T>(`${endpoint}`, body, options);
  }

  put<T>(endpoint: string, body: any, options?: {headers?: HttpHeaders}): Observable<T> {
    return this.http.put<T>(`${endpoint}`, body, options);
  }

  delete<T>(endpoint: string, options?: {headers?: HttpHeaders}): Observable<T> {
    return this.http.delete<T>(`${endpoint}`, options);
  }
}
