import { Routes } from '@angular/router';
import { AdminGuard } from '../../core/guards/admin.guard';
import { AuthGuard } from '../../core/guards/auth.guard';

export const ALERTS_ROUTES: Routes = [
  {
    path: 'alert-sources',
    canActivate: [AuthGuard, AdminGuard],
    loadComponent: () => import('./alert-sources-page.component').then(m => m.AlertSourcesPageComponent),
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => import('./alerts-page.component').then(m => m.AlertsPageComponent),
  },
];
