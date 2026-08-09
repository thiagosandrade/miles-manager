import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./auth-management.component')
        .then(c => c.AuthManagementComponent)
  }
];