import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CompareFlightRequest,
  FlightComparisonDto
} from './models/flight-comparison.model';

@Injectable({
  providedIn: 'root'
})
export class CalculatorApiService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = `${environment.apiBaseUrl}/calculator`;

  compare(request: CompareFlightRequest): Observable<FlightComparisonDto> {

    return this.http.post<FlightComparisonDto>(
      `${this.apiUrl}/compare-flight`,
      request
    );

  }

}