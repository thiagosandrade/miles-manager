import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig } from "@angular/core";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";
import { ErrorInterceptor } from "./core/interceptors/error.interceptor";
import { JwtInterceptor } from "./core/interceptors/jwt.interceptor";
import { LoadingInterceptor } from "./core/interceptors/loading.interceptor";
import { provideTranslation } from "./core/i18n/translation.config";

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    provideRouter(routes),
    provideTranslation
  ]
};
