import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const PROGRAM_BALANCES_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => 
      import('./program-balances-page.component').then(
        (m) => m.ProgramBalancesPageComponent)
  }
];
