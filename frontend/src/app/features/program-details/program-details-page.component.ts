import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslatePipe } from '@ngx-translate/core';

import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { ProgramDetailsApiService } from './program-details-api.service';
import { ProgramActivity, ProgramDetails, ProgramRedemption } from './models/program-details.model';

@Component({
  selector: 'app-program-details-page',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    MatFormFieldModule,
    MatIconModule,
    MatPaginatorModule,
    MatSelectModule,
    MatTableModule,
    MatTabsModule,
    TranslatePipe,
    AppTableComponent
  ],
  templateUrl: './program-details-page.component.html',
  styleUrls: ['./program-details-page.component.scss']
})
export class ProgramDetailsPageComponent implements OnInit {
  private readonly programsService = inject(ProgramsApiService);
  private readonly detailsService = inject(ProgramDetailsApiService);

  readonly programs = signal<Program[]>([]);
  readonly selectedProgramId = signal<string | null>(null);
  readonly details = signal<ProgramDetails | null>(null);
  readonly loadingPrograms = signal(false);
  readonly loadingDetails = signal(false);
  readonly error = signal(false);

  readonly activityData = signal<ProgramActivity[]>([]);
  readonly pagedActivity = signal<ProgramActivity[]>([]);
  readonly redemptionData = signal<ProgramRedemption[]>([]);
  readonly pagedRedemptions = signal<ProgramRedemption[]>([]);

  private activityPageIndex = 0;
  private activityPageSize = 5;
  private redemptionPageIndex = 0;
  private redemptionPageSize = 5;

  readonly activityColumns = ['date', 'description', 'type', 'miles', 'balance'];
  readonly redemptionColumns = ['flightDate', 'route', 'passengers', 'milesUsed'];

  ngOnInit(): void {
    this.loadPrograms();
  }

  async loadPrograms(): Promise<void> {
    this.loadingPrograms.set(true);
    this.error.set(false);

    try {
      const result = await this.programsService.getPrograms();
      this.programs.set(result.programs);

      if (result.programs.length > 0) {
        this.selectProgram(result.programs[0].id);
      }
    } catch {
      this.error.set(true);
    } finally {
      this.loadingPrograms.set(false);
    }
  }

  selectProgram(programId: string): void {
    if (!programId || programId === this.selectedProgramId()) {
      return;
    }

    this.selectedProgramId.set(programId);
    this.details.set(null);
    this.activityData.set([]);
    this.pagedActivity.set([]);
    this.redemptionData.set([]);
    this.pagedRedemptions.set([]);
    this.activityPageIndex = 0;
    this.redemptionPageIndex = 0;
    this.loadDetails(programId);
  }

  private loadDetails(programId: string): void {
    this.loadingDetails.set(true);
    this.error.set(false);

    this.detailsService.getProgramDetails(programId).subscribe({
      next: result => {
        this.details.set(result);
        this.activityData.set(result.activity);
        this.redemptionData.set(result.redemptions);
        this.updateActivityPage();
        this.updateRedemptionPage();
        this.loadingDetails.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loadingDetails.set(false);
      }
    });
  }

  onActivityPageChange(event: PageEvent): void {
    this.activityPageIndex = event.pageIndex;
    this.activityPageSize = event.pageSize;
    this.updateActivityPage();
  }

  onRedemptionPageChange(event: PageEvent): void {
    this.redemptionPageIndex = event.pageIndex;
    this.redemptionPageSize = event.pageSize;
    this.updateRedemptionPage();
  }

  private updateActivityPage(): void {
    const start = this.activityPageIndex * this.activityPageSize;
    this.pagedActivity.set(
      this.activityData().slice(start, start + this.activityPageSize)
    );
  }

  private updateRedemptionPage(): void {
    const start = this.redemptionPageIndex * this.redemptionPageSize;
    this.pagedRedemptions.set(
      this.redemptionData().slice(start, start + this.redemptionPageSize)
    );
  }
}
