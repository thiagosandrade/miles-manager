import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const TRANSFERS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => 
      import('./transfers-page.component').then(
        (m) => m.TransfersPageComponent)
  }
];
