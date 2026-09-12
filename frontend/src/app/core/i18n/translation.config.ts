import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';

import { provideTranslateService } from '@ngx-translate/core';

import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';

export const provideTranslation: EnvironmentProviders =
  makeEnvironmentProviders([

    provideTranslateService({

      loader: provideTranslateHttpLoader({

        resources: [
          './assets/i18n/auth/',
          './assets/i18n/common/',
          './assets/i18n/dashboard/',
          './assets/i18n/users/',
          './assets/i18n/programs/',
          './assets/i18n/program-details/',
          './assets/i18n/program-factors/',
          './assets/i18n/balances/',
          './assets/i18n/purchases/',
          './assets/i18n/transfers/',
          './assets/i18n/redemptions/',
          './assets/i18n/user-roles/',
          './assets/i18n/calculator/'
        ]

      }),

      fallbackLang: 'en',
      lang: 'en'

    })

  ]);
