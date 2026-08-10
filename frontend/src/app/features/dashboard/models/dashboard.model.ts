export interface DashboardDto {
  totalMiles: number;
  totalInvestment: number;
  averageCostPerThousand: number;
  currentMarketValue: number;
  currentProfit: number;
  currentLoss: number;
  programs: ProgramDto[];
  recentPurchases: PurchaseDto[];
  recentTransfers: TransferDto[];
  recentRedemptions: RedemptionDto[];
}

export interface ProgramDto {
  programId: string;
  programName: string;
  balance: number;
  referencePricePerThousand: number;
  currentValue: number;
}

export interface PurchaseDto {
  programId: string;
  programName: string;
  partner?: string;
  description?: string;
  amountPaid: number;
  milesPurchased: number;
  bonusPercentage: number;
  purchaseDate: string; 
  purchaseStatus: number;
}

export interface TransferDto {
  id: string;
  originProgram: string;
  destinationProgram: string;
  milesSent: number;
  milesReceived: number;
  bonusPercentage: number;
  transferDate: string; 
  costPerThousandOrigin: number; 
  costPerThousandDestination: number; 
}

export interface RedemptionDto {
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