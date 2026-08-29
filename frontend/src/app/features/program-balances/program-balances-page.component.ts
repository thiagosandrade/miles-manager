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

  balances = signal<ProgramBalance[]>([]);
  pagedBalances = signal<ProgramBalance[]>([]);
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

        this.balances.set(balances);
        this.updatePagedBalances();
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
    this.updatePagedBalances();
  }

  private updatePagedBalances(): void {
    const values = this.balances();
    const start = this.pageIndex * this.pageSize;
    this.pagedBalances.set(values.slice(start, start + this.pageSize));
  }

}
