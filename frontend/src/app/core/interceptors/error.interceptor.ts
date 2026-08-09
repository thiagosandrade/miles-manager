import { Injectable, inject } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SnackbarService } from '../services/snackbar.service';
import { AuthApiService } from '../../features/auth/auth-api.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private snackbarService = inject(SnackbarService);
  private authenticationService = inject(AuthApiService);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';
        if (error.status === 401) {
          // Auto logout if 401 response returned from api
          this.authenticationService.logout();
          location.reload();
        }
        if (error.error && error.error.message) {
          errorMsg = error.error.message;
        } else {
          errorMsg = error.message || 'An error occurred';
        }
        this.snackbarService.show(errorMsg, 'Error', 5000);
        return throwError(() => error);
      })
    );
  }
}
