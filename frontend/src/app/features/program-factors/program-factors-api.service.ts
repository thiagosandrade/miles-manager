import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProgramFactor } from './models/program-factor.model';

@Injectable({
  providedIn: 'root'
})
export class ProgramFactorsApiService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = `${environment.apiBaseUrl}/program-factors`;

  getAll(): Promise<ProgramFactor[]> {
    return firstValueFrom(this.http.get<ProgramFactor[]>(this.apiBaseUrl));
  }
}
