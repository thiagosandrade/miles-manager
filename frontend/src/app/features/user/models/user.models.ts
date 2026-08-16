import { UserRole } from "../../user-roles/models/user-roles.models";

export interface UserSummary {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAtUtc: string;
  roles: UserRole[];
}

export interface UserDetails extends UserSummary {
  updatedAtUtc: string | null;
  lastLoginAtUtc: string | null;
}

export interface IssuedToken {
  accessToken: string;
  expiresAtUtc: string;
}

export interface AuthSession {
  token: IssuedToken;
  user: UserSummary;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CreateUserRequest {
  email: string;
  fullName: string;
  password: string;
}
