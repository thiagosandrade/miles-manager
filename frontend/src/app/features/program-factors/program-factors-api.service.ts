import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProgramFactor } from './models/program-factor.model';

export interface ProgramFactorRequest {
  originProgramId: string;
  destinationProgramId: string;
  fromAmount: number;
  toAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProgramFactorsApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = `${environment.apiBaseUrl}/program-factors`;

  getAll(): Promise<ProgramFactor[]> {
    return firstValueFrom(this.http.get<ProgramFactor[]>(this.apiBaseUrl));
  }

  create(request: ProgramFactorRequest): Observable<ProgramFactor> {
    return this.http.post<ProgramFactor>(this.apiBaseUrl, request);
  }

  update(id: string, request: ProgramFactorRequest): Observable<ProgramFactor> {
    return this.http.put<ProgramFactor>(`${this.apiBaseUrl}/${id}`, request);
  }
}
