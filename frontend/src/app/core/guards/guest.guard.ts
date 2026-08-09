import { Injectable, inject } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable({
  providedIn: 'root'
})
export class GuestGuard implements CanActivate {
  private tokenStorage = inject(TokenStorageService);
  private router = inject(Router);

  canActivate(): boolean | UrlTree {
    const token = this.tokenStorage.getToken();
    if (token) {
      // User is logged in, redirect to dashboard
      return this.router.createUrlTree(['/dashboard']);
    }
    return true;
  }
}
