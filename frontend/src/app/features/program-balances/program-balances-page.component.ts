import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { TranslatePipe } from '@ngx-translate/core';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { ProgramBalanceService } from './program-balance.service';
import { ProgramBalance } from './models/program-balance.model';

@Component({
  selector: 'app-program-balances-page',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatInputModule, MatTableModule, AppTableComponent, TranslatePipe],
  templateUrl: './program-balances-page.component.html',
  styleUrls: ['./program-balances-page.component.scss']
})
export class ProgramBalancesPageComponent implements OnInit {
  readonly allData = signal<ProgramBalance[]>([]);
  readonly data = signal<ProgramBalance[]>([]);
  readonly pagedData = signal<ProgramBalance[]>([]);
  readonly loading = signal(false);
  readonly isRateLimitReached = signal(false);

  private pageIndex = 0;
  private pageSize = 5;
  private currentFilter = '';
  private sortActive = '';
  private sortDirection: 'asc' | 'desc' | '' = '';

  displayedColumns = ['program', 'currentValue', 'currencyCode', 'balance'];

  constructor(private programBalanceService: ProgramBalanceService) {}

  ngOnInit(): void {
    this.loadBalances();
  }

  loadBalances(): void {
    this.loading.set(true);
    this.isRateLimitReached.set(false);

    this.programBalanceService.getProgramBalances().subscribe({
      next: balances => {
        this.allData.set(balances);
        this.applyFilter(this.currentFilter);
        this.loading.set(false);
      },
      error: () => {
        this.isRateLimitReached.set(true);
        this.loading.set(false);
      }
    });
  }

  onSortChange(event: Sort): void {
    this.sortActive = event.direction ? event.active : '';
    this.sortDirection = event.direction;
    this.pageIndex = 0;
    this.applyFilter(this.currentFilter);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  applyFilter(filter: string): void {
    this.currentFilter = filter;
    const normalizedFilter = filter.trim().toLowerCase();
    const filtered = this.allData().filter(item =>
      item.programName?.toLowerCase().includes(normalizedFilter) ?? false
    );

    this.data.set(this.sortData(filtered));
    this.updatePagedData();
  }

  private sortData(values: ProgramBalance[]): ProgramBalance[] {
    if (!this.sortActive || !this.sortDirection) return values;

    return [...values].sort((left, right) => {
      const a = this.getSortValue(left);
      const b = this.getSortValue(right);
      const result = typeof a === 'string' && typeof b === 'string'
        ? a.localeCompare(b, undefined, { sensitivity: 'base' })
        : a < b ? -1 : a > b ? 1 : 0;

      return this.sortDirection === 'asc' ? result : -result;
    });
  }

  private getSortValue(balance: ProgramBalance): string | number {
    switch (this.sortActive) {
      case 'program': return balance.programName ?? '';
      case 'currentValue': return balance.currentValue ?? 0;
      case 'currencyCode': return balance.currencyCode ?? '';
      case 'balance': return balance.balance ?? 0;
      default: return '';
    }
  }

  private updatePagedData(): void {
    const values = this.data();
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(values.slice(start, start + this.pageSize));
  }
}
