import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { PageEvent } from '@angular/material/paginator';
import { TranslatePipe } from '@ngx-translate/core';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { ProgramFactorsApiService } from './program-factors-api.service';
import { ProgramFactor } from './models/program-factor.model';
import { CreateProgramFactorDialogComponent } from './create-program-factor-dialog.component';

@Component({
  selector: 'app-program-factors-page',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    MatButtonModule,
    TranslatePipe,
    AppTableComponent
  ],
  templateUrl: './program-factors-page.component.html',
  styleUrls: ['./program-factors-page.component.scss']
})
export class ProgramFactorsPageComponent implements OnInit {
  private readonly service = inject(ProgramFactorsApiService);
  private readonly snackbar = inject(SnackbarService);
  private readonly dialog = inject(MatDialog);

  readonly data = signal<ProgramFactor[]>([]);
  readonly pagedData = signal<ProgramFactor[]>([]);
  readonly loading = signal(false);
  readonly hasError = signal(false);

  readonly displayedColumns = [
    'originProgram',
    'destinationProgram',
    'factor',
    'actions'
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

  openCreateProgramFactorDialog(): void {
    const dialogRef = this.dialog.open(CreateProgramFactorDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        operation: 'create'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProgramFactors();
      }
    });
  }

  deleteProgramFactor(factor: ProgramFactor): void {
    const confirmed = window.confirm(
      `Delete transfer factor ${factor.originProgramName} → ${factor.destinationProgramName}?`
    );

    if (!confirmed) {
      return;
    }

    this.loading.set(true);

    this.service.delete(factor.id).subscribe({
      next: () => {
        this.snackbar.success('Program factor deleted successfully.', 3000);
        this.loadProgramFactors();
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.error('Could not delete program factor.', 3000);
      }
    });
  }

  openEditProgramFactorDialog(factor: ProgramFactor): void {
    const dialogRef = this.dialog.open(CreateProgramFactorDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: {
        operation: 'update',
        programFactor: factor
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProgramFactors();
      }
    });
  }

  private updatePagedData(): void {
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(this.data().slice(start, start + this.pageSize));
  }
}
