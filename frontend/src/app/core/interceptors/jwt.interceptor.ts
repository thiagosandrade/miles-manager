import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TokenStorageService } from '../services/token-storage.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  private tokenStorage = inject(TokenStorageService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.tokenStorage.getToken();
    const isExcludedUrl = req.url.endsWith('/auth/login') || req.url.endsWith('/auth/register');

    if (token && !isExcludedUrl) {
      const clonedReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token.accessToken}`
        }
      });
      return next.handle(clonedReq);
    }
    return next.handle(req);
  }
}
