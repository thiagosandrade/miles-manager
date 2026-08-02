import { Routes } from '@angular/router';

export const PROGRAMS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./programs-page.component').then((m) => m.ProgramsPageComponent)
  }
];
