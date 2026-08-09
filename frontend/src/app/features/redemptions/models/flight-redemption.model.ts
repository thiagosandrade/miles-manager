import { Program } from "../../programs/models/program.model";

export interface FlightRedemption {
  id: string;
  program: string;
  originAirport: string;
  destinationAirport: string;
  flightDate: string;
  cabinClass: number;
  passengers: number;
  milesUsed: number;
  taxes: number;
  cashPrice: number;
  effectiveCostPerThousand: number;
}

  export interface FlightRedemptionCreateRequest {
  userId: string;
  programId: string;
  OriginAirport: string;
  DestinationAirport: string;
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
  OriginAirport: string;
  DestinationAirport: string;
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
