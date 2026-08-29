import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { from } from 'rxjs';

import { DashboardApiService } from './dashboard-api.service';

import {
  DashboardDto,
  DashboardProgramDto
} from './models/dashboard.model';

import { AppLoadingSpinnerComponent }
  from '../../shared/components/app-loading-spinner/app-loading-spinner.component';

import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatButtonModule,
    MatIconModule,
    AppLoadingSpinnerComponent,
    TranslatePipe
  ],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {

  private readonly dashboardApi = inject(DashboardApiService);

  stats = signal<DashboardDto | null>(null);

  loading = signal(false);

  error = signal('');


  ngOnInit(): void {
    this.loadDashboard();
  }


  loadDashboard(): void {

    this.loading.set(true);

    this.error.set('');

    from(this.dashboardApi.getDashboard()).subscribe({

      next: response => {

        this.stats.set(response);

      },

      error: err => {

        this.error.set(
          err instanceof Error
            ? err.message
            : 'Error loading dashboard statistics'
        );

        this.loading.set(false);

      },

      complete: () => {

        this.loading.set(false);

      }

    });

  }


  formatCurrency(
    value: number,
    currencyCode?: string
  ): string {

    const currency = currencyCode?.trim() || 'USD';

    try {
      return new Intl.NumberFormat(
        undefined,
        {
          style: 'currency',
          currency,
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        }
      ).format(value);
    } catch {
      return new Intl.NumberFormat(
        undefined,
        {
          style: 'currency',
          currency: 'USD',
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        }
      ).format(value);
    }

  }


  formatMiles(value: number): string {

    return new Intl.NumberFormat(
      undefined,
      {
        maximumFractionDigits: 0
      }
    ).format(value);

  }


  formatPercentage(value: number): string {

    return new Intl.NumberFormat(
      undefined,
      {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
        signDisplay: 'always'
      }
    ).format(value) + '%';

  }


  isProfit(program: DashboardProgramDto): boolean {

    return program.unrealizedProfitLoss >= 0;

  }

}
