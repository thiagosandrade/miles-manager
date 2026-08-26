import { Routes } from '@angular/router';

import { AuthGuard } from '../../core/guards/auth.guard';

export const CALCULATOR_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./calculator.component')
        .then(m => m.CalculatorComponent)
  }
];