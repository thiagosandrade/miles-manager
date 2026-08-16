import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.routes').then(
        (m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () =>      
      import('./layout/layout.component').then(
        (m) => m.LayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then(
            (m) => m.DASHBOARD_ROUTES
          ),
      },
      {
        path: 'users',
        loadChildren: () =>
          import('./features/user/user.routes').then(
            (m) => m.USER_ROUTES),
      },
      {
        path: 'user-roles',
        canActivate: [
          AuthGuard,
          AdminGuard
        ],
        loadChildren: () =>
          import('./features/user-roles/user-roles.routes').then(
            (m) => m.USER_ROLES_ROUTES),
      },
      {
        path: 'programs',
        loadChildren: () =>
          import('./features/programs/programs.routes').then(
            (m) => m.PROGRAMS_ROUTES
          ),
      },
      {
        path: 'program-balances',
        loadChildren: () =>
          import('./features/program-balances/program-balances.routes').then(
            (m) => m.PROGRAM_BALANCES_ROUTES
          ),
      },
      {
        path: 'purchases',
        loadChildren: () =>
          import('./features/purchases/purchases.routes').then(
            (m) => m.PURCHASES_ROUTES
          ),
      },
      {
        path: 'transfers',
        loadChildren: () =>
          import('./features/transfers/transfers.routes').then(
            (m) => m.TRANSFERS_ROUTES
          ),
      },
      {
        path: 'redemptions',
        loadChildren: () =>
          import('./features/redemptions/redemptions.routes').then(
            (m) => m.REDEMPTIONS_ROUTES
          ),
      }
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];