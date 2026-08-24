import { UserRole } from "../../user-roles/models/user-roles.models";

export interface UserDetails {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  lastLoginAtUtc: string | null;
  roles: UserRole[];
}

export interface IssuedToken {
  accessToken: string;
  expiresAtUtc: string;
}

export interface AuthSession {
  token: IssuedToken;
  user: UserDetails;
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

export interface UpdateUserRequest {
  id: string;
  email: string;
  fullName: string;
  password: string;
  isActive: boolean;
}