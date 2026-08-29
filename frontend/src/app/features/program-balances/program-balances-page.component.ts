import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

import { ProgramBalanceService } from './program-balance.service';
import { ProgramBalance } from './models/program-balance.model';
import { MatInputModule } from '@angular/material/input';
import { PageEvent } from '@angular/material/paginator';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-program-balances-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    AppTableComponent,
    TranslatePipe
  ],
  templateUrl: './program-balances-page.component.html',
  styleUrls: ['./program-balances-page.component.scss']
})
export class ProgramBalancesPageComponent implements OnInit {

  readonly allData = signal<ProgramBalance[]>([]);
  readonly data = signal<ProgramBalance[]>([]);

  pagedData = signal<ProgramBalance[]>([]);
  private pageIndex = 0;
  private pageSize = 5;
  loading = signal(false);

  displayedColumns = [
    'program',
    'currentValue',
    'currencyCode',
    'balance'
  ];

  constructor(
    private programBalanceService: ProgramBalanceService
  ) {}

  ngOnInit(): void {
    this.loadBalances();
  }

  loadBalances(): void {

    this.loading.set(true);

    this.programBalanceService.getProgramBalances().subscribe({

      next: balances => {
        this.allData.set(balances);
        this.data.set(balances);
        this.updatePagedData();
        this.loading.set(false);

      },

      error: () => {

        this.loading.set(false);

      }

    });

  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  private updatePagedData(): void {
    const values = this.data();
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(values.slice(start, start + this.pageSize));
  }

  applyFilter(filter: string): void {

    const normalizedFilter = filter.trim().toLowerCase();

    const filtered = this.allData().filter(item =>
      item.programName?.toLowerCase().includes(normalizedFilter) ?? false
    );

    this.data.set(filtered);
    this.updatePagedData();

  }
}
