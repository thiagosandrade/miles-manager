export interface ProgramBalance {
  id: string;
  userId: string;
  programId: string;
  programName: string;
  referencePricePerThousand: number;
  balance: number;
  currentValue: number;
  currencyCode: string;
}

