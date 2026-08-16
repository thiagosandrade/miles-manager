import { Injectable, inject } from "@angular/core";
import { CanActivate, Router, UrlTree } from "@angular/router";
import { TokenStorageService } from "../services/token-storage.service";

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  private readonly storage =
    inject(TokenStorageService);

  private readonly router =
    inject(Router);

  canActivate(): boolean | UrlTree {

    if (!this.storage.isAdministrator()) {

      return this.router.createUrlTree([
        '/dashboard'
      ]);

    }

    return true;

  }

}