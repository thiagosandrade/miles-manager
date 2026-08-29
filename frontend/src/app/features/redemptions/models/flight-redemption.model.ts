import { Program } from "../../programs/models/program.model";

export interface FlightRedemption {
  id: string;
  program: string;
  originAirportCode: string;
  destinationAirportCode: string;
  flightDate: string;
  cabinClass: number;
  passengers: number;
  isRoundTrip: boolean;
  milesUsed: number;
  taxes: number;
  currencyCode: string;
  cashPrice: number;
}

  export interface FlightRedemptionCreateRequest {
  userId: string;
  programId: string;
  originAirportCode: string;
  destinationAirportCode: string;
  flightDate: string;
  cabinClass: number;
  passengers: number;
  isRoundTrip: boolean;
  milesUsed: number;
  taxes: number;
  cashPrice: number;
}

export interface FlightRedemptionUpdateRequest {
  id: string;
  userId: string;
  programId: string;
  originAirportCode: string;
  destinationAirportCode: string;
  flightDate: string;
  cabinClass: number;
  passengers: number;
  isRoundTrip: boolean;
  milesUsed: number;
  taxes: number;
  cashPrice: number;
}

export interface FlightRedemptionListResponse {
  redemptions: FlightRedemption[];
  totalCount: number;
}
