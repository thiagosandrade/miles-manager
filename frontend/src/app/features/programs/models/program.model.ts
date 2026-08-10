export interface Program {
  id: string;
  name: string;
  website?: string;
  logo?: string;
  programType: number;
  referencePricePerThousand: number;
  isActive: boolean;
  description?: string;
  partnerId: string;
  programCode: string;
  status: number;
  expirationAfterMonths: number;
  expirationEnabled: boolean;
}

export interface ProgramCreateRequest {
  programCode: string;
  name: string;
  programType: number;
  website?: string | null;
  logo?: string | null;
  referencePricePerThousand: number;
  isActive?: boolean;
}

export interface ProgramUpdateRequest {
  id: string;
  programCode: string;
  name: string;
  programType: number;
  website?: string | null;
  logo?: string | null;
  referencePricePerThousand: number;
  isActive: boolean;
}

export interface ProgramListResponse {
  programs: Program[];
  totalCount: number;
}
