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

@Component({
  selector: 'app-programs-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    AppTableComponent
  ],
  templateUrl: './programs-page.component.html',
  styleUrls: ['./programs-page.component.scss']
})
export class ProgramsPageComponent implements OnInit {
  dialog = inject(MatDialog);

  data = signal<Program[]>([]);
  loading = signal(false);
  isRateLimitReached = signal(false);

  displayedColumns = [
    'name',
    'isActive',
    'actions'
  ];

  filter = '';

  constructor(
    private programsService: ProgramsApiService
  ) {}

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

      const result = await this.programsService.listPrograms({
        page: pageIndex + 1,
        pageSize,
        sort: sortField
          ? `${sortDirection === 'asc' ? '' : '-'}${sortField}`
          : undefined,
        filter: this.filter
      });

      this.data.set(result.programs);

    } catch {

      this.isRateLimitReached.set(true);

    } finally {

      this.loading.set(false);

    }

  }

  applyFilter(filter: string): void {

    this.filter = filter.trim().toLowerCase();

    this.loadPrograms();

  }

  openCreateDialog(): void {

    const dialogRef = this.dialog.open(CreateProgramDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(created => {

      if (created) {
        this.loadPrograms();
      }

    });

  }
}