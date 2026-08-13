import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';

import { provideTranslateService } from '@ngx-translate/core';

import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const provideTranslation: EnvironmentProviders =
  makeEnvironmentProviders([

    provideTranslateService({

      loader: provideTranslateHttpLoader({

        prefix: './assets/i18n/',

        suffix: '.json'

      }),

      fallbackLang: 'en',

      lang: 'en'

    })

  ]);