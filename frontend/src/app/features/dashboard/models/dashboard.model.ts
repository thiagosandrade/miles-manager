export interface DashboardDto {
  programCount: number;
  programs: DashboardProgramDto[];
  recentPurchases: PurchaseDto[];
  recentTransfers: TransferDto[];
  recentRedemptions: RedemptionDto[];
}


export interface DashboardProgramDto {
  programId: string;
  programName: string;
  currencyCode: string;
  miles: number;
  actualCostPerThousand: number;
  referencePricePerThousand: number;
  unrealizedProfitLoss: number;
  unrealizedProfitLossPercentage: number;
}


export interface PurchaseDto {
  programId: string;
  programName: string;
  partner?: string;
  description?: string;
  amountPaid: number;
  currencyCode: string;
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
  currencyCodeOrigin: string;
  currencyCodeDestination: string;
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
  currencyCode: string;
  effectiveCostPerThousand: number;
}