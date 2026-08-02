import { Routes } from '@angular/router';

export const REPORTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./reports-page.component').then((m) => m.ReportsPageComponent)
  }
];
