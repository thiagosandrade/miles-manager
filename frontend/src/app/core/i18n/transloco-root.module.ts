import { importProvidersFrom, Provider } from '@angular/core';
import {
  TranslocoLoader,
  TRANSLOCO_CONFIG,
  TRANSLOCO_LOADER,
  TranslocoModule,
  translocoConfig
} from '@ngneat/transloco';
import { HttpClient } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { Observable } from 'rxjs';

export class TranslocoHttpLoader implements TranslocoLoader {
  constructor(private http: HttpClient) { }
  getTranslation(lang: string): Observable<any> {
    return this.http.get(`/assets/i18n/${lang}.json`);
  }
}

export const translocoRootModuleProviders: Provider[] = [
  importProvidersFrom(HttpClientModule, TranslocoModule),
  {
    provide: TRANSLOCO_CONFIG,
    useValue: translocoConfig({
      availableLangs: ['en', 'pt-BR', 'pt-PT', 'es', 'fr', 'de'],
      defaultLang: 'pt-BR',
      fallbackLang: 'pt-BR',
      reRenderOnLangChange: true,
      prodMode: false
    })
  },
  {
    provide: TRANSLOCO_LOADER,
    useClass: TranslocoHttpLoader
  }
];
