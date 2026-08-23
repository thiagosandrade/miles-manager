import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

import { ProgramsApiService } from './programs-api.service';
import { Program } from './models/program.model';
import { MatDialog } from '@angular/material/dialog';
import { CreateProgramDialogComponent } from './create-program-dialog.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';
import { TokenStorageService } from '../../core/services/token-storage.service';

@Component({
  selector: 'app-programs-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    TranslatePipe,
    AppTableComponent
],
  templateUrl: './programs-page.component.html',
  styleUrls: ['./programs-page.component.scss']
})
export class ProgramsPageComponent implements OnInit {
  dialog = inject(MatDialog);
  snackbarService = inject(SnackbarService);
  
  private readonly storage = inject(TokenStorageService);
  readonly isAdministrator = this.storage.isAdministrator();
  
  data = signal<Program[]>([]);
  loading = signal(false);
  isRateLimitReached = signal(false);

  displayedColumns = [
    'name',  
    'referencePricePerThousand',
    'currencyCode',
    'isActive',
  ];

  filter = '';

  constructor(
    private programsService: ProgramsApiService
  ) {
    if(this.isAdministrator) this.displayedColumns.push('actions')
  }

  ngOnInit(): void {
    this.loadPrograms();
  }

  async loadPrograms(
    pageIndex = 0,
    pageSize = 10,
    sortField = '',
    sortDirection = ''
  ): Promise<void> {

    this.loading.set(true);
    this.isRateLimitReached.set(false);

    try {

      await this.programsService.listPrograms({
        page: pageIndex + 1,
        pageSize,
        sort: sortField
          ? `${sortDirection === 'asc' ? '' : '-'}${sortField}`
          : undefined,
        filter: this.filter
      }).then(result => {
        this.data.set(result.programs);
      });

    } catch {

      this.isRateLimitReached.set(true);
      this.snackbarService.error('Could not load programs.', 3000);

    } finally {

      this.loading.set(false);

    }

  }

  applyFilter(filter: string): void {

    this.filter = filter.trim().toLowerCase();

    this.loadPrograms();

  }

  openUpdateDialog(program: Program): void {
    const dialogRef = this.dialog.open(CreateProgramDialogComponent, {
      width: '500px',
      data: {
        operation: 'update',
        programId: program.id
      }
    });

    dialogRef.afterClosed().subscribe(updated => {

      if (updated) {
        this.snackbarService.success('Program updated succesfully.', 3000);
        this.loadPrograms();
      }

    });
    
  }

  openCreateDialog(): void {

    const dialogRef = this.dialog.open(CreateProgramDialogComponent, {
      width: '500px',
      data: {
        operation: 'create'
      }
    });

    dialogRef.afterClosed().subscribe(created => {

      if (created) {
        this.snackbarService.success('Program created succesfully.', 3000);
        this.loadPrograms();
      }
    });

  }

  deleteProgram(program: Program): void {
  
      this.programsService.deleteProgram(program.id).subscribe({
          next: () => {
            this.snackbarService.success('Program updated succesfully.', 3000);
            this.loadPrograms();
            this.loading.set(false)
          },
          error: () => {
            this.snackbarService.error('Could not create program.', 3000);
            this.loading.set(false)
          } 
        });
  
    }
}