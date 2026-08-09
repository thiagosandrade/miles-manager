import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const PROGRAMS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () => 
      import('./programs-page.component').then(
        (m) => m.ProgramsPageComponent)
  }
];
