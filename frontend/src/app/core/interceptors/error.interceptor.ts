import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";
import { TokenStorageService } from "../services/token-storage.service";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  private router = inject(Router);
  private storage = inject(TokenStorageService);

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    return next.handle(req).pipe(

      catchError((error: HttpErrorResponse) => {

        if (error.status === 401 || error.status === 403) {

          this.storage.logoutToken();

          this.router.navigate(['/auth']);

          return throwError(() => error);
        }

        return throwError(() => error);

      })

    );
  }
}