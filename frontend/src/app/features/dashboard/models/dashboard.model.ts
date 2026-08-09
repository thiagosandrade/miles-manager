// Since the backend DTO is not provided, a stub interface
export interface Dashboard {
  totalMiles: number;
  totalPrograms: number;
  totalPurchases: number;
  totalTransfers: number;
  totalRedemptions: number;
}

export interface DashboardDto {
  totalMiles: number;
  totalInvestment: number;
  averageCostPerThousand: number;
  currentMarketValue: number;
  currentProfit: number;
  currentLoss: number;
  programs: any[];
  recentPurchases: any[];
  recentTransfers: any[];
  recentRedemptions: any[];
}

