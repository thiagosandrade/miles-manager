import { Injectable, inject } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  private tokenStorage = inject(TokenStorageService);
  private router = inject(Router);

  canActivate(): boolean | UrlTree {
    const token = this.tokenStorage.getToken();
    if (token) {
      return true;
    }
    return this.router.createUrlTree(['/auth']);
  }
}
