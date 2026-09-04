import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'sign-up',
    loadComponent: () =>
      import('./sign-up.component')
        .then(c => c.SignUpComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./forgot-password.component')
        .then(c => c.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./reset-password.component')
        .then(c => c.ResetPasswordComponent)
  },
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./auth-management.component')
        .then(c => c.AuthManagementComponent)
  }
];
