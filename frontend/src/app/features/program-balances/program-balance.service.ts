import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiService } from '../../core/services/api.service';
import { ProgramBalance } from './models/program-balance.model';

@Injectable({
  providedIn: 'root'
})
export class ProgramBalanceService {
  private api = inject(ApiService);
  private apiBaseUrl = `${environment.apiBaseUrl}/program-balances`;

  getProgramBalances(): Observable<ProgramBalance[]> {
    return this.api.get<ProgramBalance[]>(`${this.apiBaseUrl}`);
  }

  getProgramBalance(id: string): Observable<ProgramBalance> {
    return this.api.get<ProgramBalance>(`${this.apiBaseUrl}/${id}`);
  }

  createProgramBalance(balance: ProgramBalance): Observable<ProgramBalance> {
    return this.api.post<ProgramBalance>(`${this.apiBaseUrl}`, balance);
  }

  updateProgramBalance(balance: ProgramBalance): Observable<ProgramBalance> {
    return this.api.put<ProgramBalance>(`${this.apiBaseUrl}/${balance.id}`, balance);
  }

  deleteProgramBalance(id: string): Observable<void> {
    return this.api.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
