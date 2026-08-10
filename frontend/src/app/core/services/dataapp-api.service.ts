import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface AppDataDto {
  airports : AirportsDTO[]
}

export interface AirportsDTO {
    iataCode: string,
    name: string,
    city: string,
    country: string
}

@Injectable({
  providedIn: 'root'
})
export class LookupService {

  private readonly apiBaseUrl = `${environment.apiBaseUrl}/lookup`;

  constructor(private http: HttpClient) {}

  private readonly airportsCacheKey = 'lookup-airports';

  async getAirports(): Promise<AirportsDTO[]> {

    const cached = localStorage.getItem(this.airportsCacheKey);

    if (cached) {
      return JSON.parse(cached) as AirportsDTO[];
    }

    const airports = (await firstValueFrom(
      this.http.get<AppDataDto>(`${this.apiBaseUrl}/airports`)
    )).airports;

    localStorage.setItem(
      this.airportsCacheKey,
      JSON.stringify(airports)
    );

    return airports;

  }

}
