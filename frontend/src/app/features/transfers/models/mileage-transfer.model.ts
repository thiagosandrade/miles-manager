import { Program } from '../../programs/models/program.model';

export interface MileageTransfer {
  id: string;
  userId: string;
  originProgramId: string;
  destinationProgramId: string;
  milesSent: number;
  bonusPercentage: number;
  milesReceived: number;
  costPerThousandOrigin: number;
  costPerThousandDestination: number;
  transferDate: string; // ISO string date
  originProgram?: Program;
  destinationProgram?: Program;
}

export interface Transfer {
  id: string;
  fromProgramId: string;
  toProgramId: string;
  amount: number;
  transferDate: string;
  createdAt: string;
  updatedAt: string;
}

export interface TransferCreateRequest {
  fromProgramId: string;
  toProgramId: string;
  amount: number;
  transferDate: string;
}

export interface TransferUpdateRequest {
  fromProgramId: string;
  toProgramId: string;
  amount: number;
  transferDate: string;
}

export interface TransferListResponse {
  transfers: Transfer[];
  totalCount: number;
}
