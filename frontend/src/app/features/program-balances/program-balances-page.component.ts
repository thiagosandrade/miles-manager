import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

import { ProgramBalanceService } from './program-balance.service';
import { ProgramBalance } from './models/program-balance.model';
import { MatInputModule } from '@angular/material/input';
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
        this.loading.set(false);

      },

      error: () => {

        this.loading.set(false);

      }

    });

  }

}
