import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiService } from '../../core/services/api.service';
import { FlightRedemption, FlightRedemptionCreateRequest, FlightRedemptionUpdateRequest } from './models/flight-redemption.model';

@Injectable({ providedIn: 'root' })
export class FlightRedemptionService {
  private api = inject(ApiService);
  private apiBaseUrl = `${environment.apiBaseUrl}/flight-redemptions`;

  getFlightRedemptions(): Observable<FlightRedemption[]> {
    return this.api.get<FlightRedemption[]>(this.apiBaseUrl);
  }

  getFlightRedemption(id: string): Observable<FlightRedemption> {
    return this.api.get<FlightRedemption>(`${this.apiBaseUrl}/${id}`);
  }

  createFlightRedemption(redemption: FlightRedemptionCreateRequest): Observable<FlightRedemption> {
    return this.api.post<FlightRedemption>(this.apiBaseUrl, redemption);
  }

  updateFlightRedemption(redemption: FlightRedemptionUpdateRequest): Observable<FlightRedemption> {
    return this.api.put<FlightRedemption>(`${this.apiBaseUrl}/${redemption.id}`, redemption);
  }

  deleteFlightRedemption(id: string): Observable<void> {
    return this.api.delete<void>(`${this.apiBaseUrl}/${id}`);
  }
}
