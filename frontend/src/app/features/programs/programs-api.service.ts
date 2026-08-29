import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom, map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProgramListResponse, Program, ProgramCreateRequest, ProgramUpdateRequest } from './models/program.model';

@Injectable({
  providedIn: 'root'
})
export class ProgramsApiService {

  private apiBaseUrl = `${environment.apiBaseUrl}/programs`;

  constructor(private http: HttpClient) {}

  async getPrograms(): Promise<ProgramListResponse> {

    const list = await firstValueFrom(this.http.get<Program[]>(`${this.apiBaseUrl}`));
    return { programs: list, totalCount: list.length };
  }

  getProgram(id: string): Observable<Program> {
    return this.http.get<Program>(`${this.apiBaseUrl}/${id}`);
  }

  createProgram(request: ProgramCreateRequest): Observable<Program> {
    return this.http.post<Program>(`${this.apiBaseUrl}`, request);
  }

  updateProgram(id: string, request: ProgramUpdateRequest): Observable<Program> {
    return this.http.put<Program>(`${this.apiBaseUrl}/${id}`, request);
  }

  deleteProgram(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
