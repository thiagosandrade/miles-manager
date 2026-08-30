import { Routes } from '@angular/router';
import { AdminGuard } from '../../core/guards/admin.guard';

export const PROGRAM_FACTORS_ROUTES: Routes = [
  {
    path: '',
    canActivate: [AdminGuard],
    loadComponent: () =>
      import('./program-factors-page.component').then(
        m => m.ProgramFactorsPageComponent
      )
  }
];
