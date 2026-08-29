import { Program } from '../../programs/models/program.model';

export interface MileageTransfer {
  id: string;
  userId: string;
  originProgram: string;
  destinationProgram: string;
  milesSent: number;
  bonusPercentage: number;
  milesReceived: number;
  transferDate: string;
}

export interface TransferCreateRequest {
  userId: string;
  originProgramId: string;
  destinationProgramId: string;
  milesSent: number;
  bonusPercentage: number;
  milesReceived: number;
  transferDate: string;
  originProgram?: Program;
  destinationProgram?: Program;
}

export interface TransferUpdateRequest {
  id: string;
  userId: string;
  originProgramId: string;
  destinationProgramId: string;
  milesSent: number;
  bonusPercentage: number;
  milesReceived: number;
  transferDate: string;
  originProgram?: Program;
  destinationProgram?: Program;
}

export interface TransferListResponse {
  transfers: MileageTransfer[];
  totalCount: number;
}
