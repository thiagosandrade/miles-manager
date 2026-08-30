import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { PageEvent } from '@angular/material/paginator';
import { TranslatePipe } from '@ngx-translate/core';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { ProgramFactorsApiService } from './program-factors-api.service';
import { ProgramFactor } from './models/program-factor.model';

@Component({
  selector: 'app-program-factors-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatTableModule,
    TranslatePipe,
    AppTableComponent
  ],
  templateUrl: './program-factors-page.component.html',
  styleUrls: ['./program-factors-page.component.scss']
})
export class ProgramFactorsPageComponent implements OnInit {
  private readonly service = inject(ProgramFactorsApiService);
  private readonly snackbar = inject(SnackbarService);

  readonly data = signal<ProgramFactor[]>([]);
  readonly pagedData = signal<ProgramFactor[]>([]);
  readonly loading = signal(false);
  readonly hasError = signal(false);

  readonly displayedColumns = [
    'originProgram',
    'destinationProgram',
    'factor'
  ];

  private pageIndex = 0;
  private pageSize = 10;

  ngOnInit(): void {
    this.loadProgramFactors();
  }

  async loadProgramFactors(): Promise<void> {
    this.loading.set(true);
    this.hasError.set(false);

    try {
      const result = await this.service.getAll();
      this.data.set(result);
      this.pageIndex = 0;
      this.updatePagedData();
    } catch {
      this.hasError.set(true);
      this.snackbar.error('Could not load program factors.', 3000);
    } finally {
      this.loading.set(false);
    }
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  private updatePagedData(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(this.data().slice(start, start + this.pageSize));
  }
}
