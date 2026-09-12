export interface ProgramActivity {
  date: string;
  description: string;
  type: 'earned' | 'redeemed';
  miles: number;
  balance: number;
}

export interface ProgramRedemption {
  id: string;
  flightDate: string;
  originAirportCode: string;
  destinationAirportCode: string;
  milesUsed: number;
  passengers: number;
  isRoundTrip: boolean;
}

export interface ProgramDetails {
  programId: string;
  programName: string;
  activity: ProgramActivity[];
  redemptions: ProgramRedemption[];
}
