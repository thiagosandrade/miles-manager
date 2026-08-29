import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";
import { TokenStorageService } from "../services/token-storage.service";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  private router = inject(Router);
  private storage = inject(TokenStorageService);
  private readonly snackBar = inject(MatSnackBar);
  
  intercept(req: HttpRequest<any>, next: HttpHandler) {

    return next.handle(req).pipe(

      catchError((error: HttpErrorResponse) => {

        if (error.status === 401) {

          this.storage.logoutToken();

          this.router.navigate(['/auth']);

          return throwError(() => error);
        }

        if (error.status === 403) {

          this.snackBar.open(
            'You do not have permission to perform this action.',
            'Close',
            {
              duration: 5000,
              horizontalPosition: 'right',
              verticalPosition: 'top'
            }
          );

          return throwError(() => error);
        }

        return throwError(() => error);

      })

    );
  }
}