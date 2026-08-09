
export interface AuthSession {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: AuthUser;
}

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

export interface Authentication {
  accessToken: string;
  refreshToken?: string;
  user: {
    id: string;
    email: string;
    fullName: string;
  };
}