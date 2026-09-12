import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProgramDetails } from './models/program-details.model';

@Injectable({ providedIn: 'root' })
export class ProgramDetailsApiService {
  private readonly apiBaseUrl = `${environment.apiBaseUrl}/program-details`;

  constructor(private readonly http: HttpClient) {}

  getProgramDetails(programId: string): Observable<ProgramDetails> {
    return this.http.get<ProgramDetails>(`${this.apiBaseUrl}/${programId}`);
  }
}
