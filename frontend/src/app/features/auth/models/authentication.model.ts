
export interface UserToken {
  accessToken: string;
  expiresAtUtc: number;
}

export interface AuthUser {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
  isActive: boolean;
  lastLoginAtUtc: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: UserToken;
  user: AuthUser;
}