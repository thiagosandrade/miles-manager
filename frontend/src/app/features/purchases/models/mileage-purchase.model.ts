export interface MileagePurchase {
  id: string;
  programName: string;
  partner?: string;
  description?: string;
  amountPaid: number;
  currencyCode: string;
  milesPurchased: number;
  purchaseDate: string; 
  purchaseStatus: number;
}

export interface PurchaseCreateRequest {
  userId: string;
  programId: string;
  partner?: string;
  description?: string;
  amountPaid: number;
  milesPurchased: number;
  purchaseDate: string; 
  purchaseStatus: number;
}

export interface PurchaseUpdateRequest {
  id: string;
  userId: string;
  programId: string;
  partner?: string;
  description?: string;
  amountPaid: number;
  milesPurchased: number;
  purchaseDate: string; 
  purchaseStatus: number;
}

export interface PurchaseListResponse {
  purchases: MileagePurchase[];
  totalCount: number;
}
