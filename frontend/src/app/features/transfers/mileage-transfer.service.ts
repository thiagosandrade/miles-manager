import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { MileageTransfer } from './models/mileage-transfer.model';
import { environment } from '../../../environments/environment';
import { ApiService } from '../../core/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class MileageTransferService {
  private api = inject(ApiService);
  private apiBaseUrl = `${environment.apiBaseUrl}/mileage-transfers`;

  getMileageTransfers(): Observable<MileageTransfer[]> {
    return this.api.get<MileageTransfer[]>(`${this.apiBaseUrl}`);
  }

  getMileageTransfer(id: string): Observable<MileageTransfer> {
    return this.api.get<MileageTransfer>(`${this.apiBaseUrl}/${id}`);
  }

  createMileageTransfer(transfer: MileageTransfer): Observable<MileageTransfer> {
    return this.api.post<MileageTransfer>(`${this.apiBaseUrl}`, transfer);
  }

  updateMileageTransfer(transfer: MileageTransfer): Observable<MileageTransfer> {
    return this.api.put<MileageTransfer>(`${this.apiBaseUrl}/${transfer.id}`, transfer);
  }

  deleteMileageTransfer(id: string): Observable<void> {
    return this.api.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
