import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'sign-up',
    loadComponent: () =>
      import('./sign-up.component')
        .then(c => c.SignUpComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./auth-management.component')
        .then(c => c.AuthManagementComponent)
  }
];
