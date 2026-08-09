import { Injectable } from '@angular/core';
import { UserToken } from '../../features/auth/models/authentication.model';

const TOKEN_KEY = 'auth-token';
const USER_KEY = 'auth-user';

@Injectable({
  providedIn: 'root'
})
export class TokenStorageService {
  public signOut(): void {
    window.sessionStorage.clear();
  }

  public saveToken(token: UserToken): void {
    window.sessionStorage.removeItem(TOKEN_KEY);
    window.sessionStorage.setItem(TOKEN_KEY, JSON.stringify(token));
  }

  public getToken(): UserToken | null {
    const usertoken = window.sessionStorage.getItem(TOKEN_KEY);
    if (usertoken) {
      return JSON.parse(usertoken);
    }
    return null;
  }

  public saveUser(user: any): void {
    window.sessionStorage.removeItem(USER_KEY);
    window.sessionStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  public getUser(): any {
    const user = window.sessionStorage.getItem(USER_KEY);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }
}
