import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const USER_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => import('./user-management.component').then(m => m.UserManagementComponent)
  },
  {
    path: ':id',
    canActivate: [AuthGuard],
    loadComponent: () => import('./user-details.component').then(c => c.UserDetailsComponent)
  }
];
