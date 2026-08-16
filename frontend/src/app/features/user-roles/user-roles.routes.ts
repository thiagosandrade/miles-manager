import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { AdminGuard } from '../../core/guards/admin.guard';

export const USER_ROLES_ROUTES: Routes = [
  {
    path: '',
    canActivate: [
          AuthGuard,
          AdminGuard
        ],
    loadComponent: () => import('./user-roles.component').then(m => m.UserRolesComponent)
  },
  {
    path: ':id',
    canActivate: [AuthGuard],
    loadComponent: () => import('./edit-user-roles-dialog.component').then(c => c.EditUserRolesDialogComponent)
  }
];
