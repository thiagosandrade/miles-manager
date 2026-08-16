import { Injectable } from '@angular/core';
import { UserToken } from '../../features/auth/models/authentication.model';
import { jwtDecode } from 'jwt-decode';

const TOKEN_KEY = 'auth-token';
const USER_KEY = 'auth-user';

interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  role?: string | string[];
  exp: number;
}

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  
  public saveToken(token: UserToken): void {

    localStorage.setItem(TOKEN_KEY, JSON.stringify(token));

    const jwt = jwtDecode<JwtPayload>(token.accessToken);

    const roles =
      Array.isArray(jwt.role)
        ? jwt.role
        : jwt.role
          ? [jwt.role]
          : [];

    localStorage.setItem(
      USER_KEY,
      JSON.stringify({

        id: jwt.sub,
        email: jwt.email,
        fullName: jwt.name,
        roles

      }));

  }

  public getToken(): UserToken | null {
    const value = localStorage.getItem(TOKEN_KEY);

    return value
      ? JSON.parse(value)
      : null;

  }

  public logoutToken(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  public getUser(): any {
    const value = localStorage.getItem(USER_KEY);

    return value
      ? JSON.parse(value)
      : null;
  }

  isAdministrator(): boolean {

    const user = this.getUser();

    return user?.roles?.includes('Administrator') ?? false;

  }
}
