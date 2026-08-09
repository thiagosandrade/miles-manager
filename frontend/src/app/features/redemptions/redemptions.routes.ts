import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const REDEMPTIONS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => 
      import('./redemptions-page.component').then(
        (m) => m.RedemptionsPageComponent)
  }
];
