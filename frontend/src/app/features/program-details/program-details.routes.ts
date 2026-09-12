import { Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';

export const PROGRAM_DETAILS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    loadComponent: () =>
      import('./program-details-page.component').then(
        (m) => m.ProgramDetailsPageComponent)
  }
];
