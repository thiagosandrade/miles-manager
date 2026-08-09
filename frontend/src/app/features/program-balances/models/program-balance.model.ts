import { Program } from "../../programs/models/program.model";

export interface ProgramBalance {
  id: string;
  userId: string;
  programId: string;
  balance: number;
  program?: Program;
}

