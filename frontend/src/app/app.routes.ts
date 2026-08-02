import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./shell/app-shell.component').then((m) => m.AppShellComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES)
      },
      {
        path: 'users',
        loadChildren: () => import('./user/user.routes').then((m) => m.USER_ROUTES)
      },
      {
        path: 'programs',
        loadChildren: () => import('./programs/programs.routes').then((m) => m.PROGRAMS_ROUTES)
      },
      {
        path: 'reports',
        loadChildren: () => import('./reports/reports.routes').then((m) => m.REPORTS_ROUTES)
      },
      {
        path: 'settings',
        loadChildren: () => import('./settings/settings.routes').then((m) => m.SETTINGS_ROUTES)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
