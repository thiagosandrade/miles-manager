import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const PURCHASES_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => 
      import('./purchases-page.component').then(
        (m) => m.PurchasesPageComponent)
  }
];
