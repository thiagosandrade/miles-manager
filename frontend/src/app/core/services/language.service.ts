import { Injectable, inject } from '@angular/core';

import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  private translate = inject(TranslateService);

  readonly supportedLanguages = [
    'en',
    'pt-BR',
    'pt-PT',
    'es',
    'fr',
    'de'
  ];

  constructor() {

    this.translate.addLangs(this.supportedLanguages);

    const savedLanguage = localStorage.getItem('language');

    if (savedLanguage) {

      this.translate.use(savedLanguage);

    } else {

      const browser =
        this.translate.getBrowserCultureLang() ??
        this.translate.getBrowserLang() ??
        'en';

      const language = this.supportedLanguages.includes(browser)
        ? browser
        : 'en';

      this.translate.use(language);

      localStorage.setItem('language', language);

    }

  }

  get currentLanguage(): string {

    return this.translate.getCurrentLang() ?? 'en';

  }

  changeLanguage(language: string): void {

    this.translate.use(language);

    localStorage.setItem('language', language);

  }

}