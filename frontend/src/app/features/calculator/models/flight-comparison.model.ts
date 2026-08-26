export interface CompareFlightRequest {
  programId: string;
  milesRequired: number;
  taxes: number;
  cashPrice: number;
}

export interface FlightComparisonDto {
  programId: string;
  programName: string;
  currencyCode: string;

  milesRequired: number;
  taxes: number;
  cashPrice: number;

  referencePricePerThousand: number;
  milesValue: number;
  totalMilesCost: number;

  savings: number;
  effectiveValuePerThousand: number;

  isMilesBetter: boolean;
}