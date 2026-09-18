import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { TranslatePipe } from '@ngx-translate/core';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { Alert } from './models/alert.model';
import { AlertSource } from './models/alert-source.model';
import { AlertsApiService } from './alerts-api.service';

@Component({
  selector: 'app-alerts-page',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatTableModule, TranslatePipe, AppTableComponent],
  templateUrl: './alerts-page.component.html',
  styleUrls: ['./alerts-page.component.scss'],
})
export class AlertsPageComponent implements OnInit {
  private readonly api = inject(AlertsApiService);
  private readonly snackbar = inject(SnackbarService);

  readonly allData = signal<Alert[]>([]);
  readonly data = signal<Alert[]>([]);
  readonly pagedData = signal<Alert[]>([]);
  readonly loading = signal(false);
  readonly hasError = signal(false);
  readonly sources = signal<AlertSource[]>([]);
  readonly selectedCountry = signal('');
  readonly countries = computed(() =>
    [...new Set(this.sources().map(source => source.country.trim()).filter(Boolean))]
      .sort((a, b) => a.localeCompare(b)),
  );

  readonly displayedColumns = ['source', 'title', 'chips', 'publishedDate', 'url'];

  sortActive = 'publishedDate';
  sortDirection: 'asc' | 'desc' | '' = 'desc';
  private pageIndex = 0;
  private pageSize = 10;

  ngOnInit(): void {
    this.loadAlerts();
    this.loadSources();
  }

  loadAlerts(): void {
    this.loading.set(true);
    this.hasError.set(false);

    this.api.getAlerts().subscribe({
      next: alerts => {
        this.allData.set(alerts);
        this.applySort();
        this.loading.set(false);
      },
      error: () => {
        this.hasError.set(true);
        this.loading.set(false);
        this.snackbar.error('Could not load alerts.', 3000);
      },
    });
  }

  loadSources(): void {
    this.api.getAlertSources().subscribe({
      next: sources => this.sources.set(sources),
      error: () => this.sources.set([]),
    });
  }

  sourceTitle(alert: Alert): string {
    const source = this.sources().find(source => this.sameHost(source.externalUrl, alert.externalUrl));
    return source?.title ?? '—';
  }

  onCountryChange(country: string): void {
    this.selectedCountry.set(country);
    this.pageIndex = 0;
    this.applySort();
  }

  onSortChange(event: Sort): void {
    this.sortActive = event.direction ? event.active : '';
    this.sortDirection = event.direction;
    this.pageIndex = 0;
    this.applySort();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  chips(alert: Alert): string[] {
    return alert.chips
      ? alert.chips.split(',').map(value => value.trim()).filter(Boolean)
      : [];
  }

  trackById(_: number, item: Alert): string {
    return item.id;
  }

  private sameHost(leftUrl: string, rightUrl: string): boolean {
    try {
      return new URL(leftUrl).host.toLowerCase() === new URL(rightUrl).host.toLowerCase();
    } catch {
      return false;
    }
  }

  private applySort(): void {
    const filtered = this.selectedCountry()
      ? this.allData().filter(alert => alert.country === this.selectedCountry())
      : this.allData();

    const sorted = [...filtered].sort((left, right) => {
      const leftValue = this.getSortValue(left, this.sortActive);
      const rightValue = this.getSortValue(right, this.sortActive);

      if (leftValue === rightValue) return 0;
      if (leftValue === null || leftValue === undefined) return 1;
      if (rightValue === null || rightValue === undefined) return -1;

      const comparison = typeof leftValue === 'string' && typeof rightValue === 'string'
        ? leftValue.localeCompare(rightValue, undefined, { sensitivity: 'base' })
        : leftValue < rightValue ? -1 : 1;

      return this.sortDirection === 'asc' ? comparison : -comparison;
    });

    this.data.set(sorted);
    this.updatePagedData();
  }

  private updatePagedData(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(this.data().slice(start, start + this.pageSize));
  }

  private getSortValue(alert: Alert, column: string): string | null {
    switch (column) {
      case 'title':
        return alert.title;
      case 'source':
        return this.sourceTitle(alert);
      case 'chips':
        return alert.chips ?? null;
      case 'publishedDate':
        return alert.publishedDate ?? alert.createdAtUtc;
      default:
        return null;
    }
  }
}
