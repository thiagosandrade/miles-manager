import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';

import { provideTranslateService } from '@ngx-translate/core';

import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const provideTranslation: EnvironmentProviders =
  makeEnvironmentProviders([

    provideTranslateService({

      loader: provideTranslateHttpLoader({

        resources: [

          './assets/i18n/',

          './assets/i18n/auth/'

        ]

      }),

      fallbackLang: 'en',

      lang: 'en'

    })

  ]);
