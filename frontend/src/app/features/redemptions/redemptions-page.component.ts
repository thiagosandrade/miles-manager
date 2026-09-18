import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';

import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { FlightRedemptionService } from './flight-redemption.service';
import { FlightRedemption, FlightRedemptionCreateRequest, FlightRedemptionUpdateRequest } from './models/flight-redemption.model';
import { CreateRedemptionDialogComponent, RedemptionDialogData } from './create-redemption-dialog.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { formatDateOnly } from '../../shared/date-utils';

@Component({
  selector: 'app-redemptions-page',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatInputModule, AppTableComponent, MatIconModule, MatTableModule, TranslatePipe],
  templateUrl: './redemptions-page.component.html',
  styleUrls: ['./redemptions-page.component.scss']
})
export class RedemptionsPageComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  readonly allData = signal<FlightRedemption[]>([]);
  readonly data = signal<FlightRedemption[]>([]);
  readonly pagedData = signal<FlightRedemption[]>([]);
  readonly loading = signal(false);
  readonly isRateLimitReached = signal(false);

  private pageIndex = 0;
  private pageSize = 5;
  private currentFilter = '';
  private sortActive = '';
  private sortDirection: 'asc' | 'desc' | '' = '';

  displayedColumns = [
    'program', 'originAirportCode', 'destinationAirportCode', 'flightDate', 'cabinClass',
    'passengers', 'milesUsed', 'taxes', 'cashPrice', 'actions'
  ];

  constructor(private redemptionService: FlightRedemptionService) {}

  ngOnInit(): void {
    this.loadRedemptions();
  }

  loadRedemptions(): void {
    this.loading.set(true);
    this.isRateLimitReached.set(false);

    this.redemptionService.getFlightRedemptions().subscribe({
      next: data => {
        this.allData.set(data);
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
    const filtered = this.allData().filter(item => {
      const values = [
        item.program,
        item.originAirportCode,
        item.destinationAirportCode,
        item.currencyCode
      ];
      return values.some(value => value?.toLowerCase().includes(normalizedFilter));
    });

    this.data.set(this.sortData(filtered));
    this.updatePagedData();
  }

  private sortData(values: FlightRedemption[]): FlightRedemption[] {
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

  private getSortValue(redemption: FlightRedemption): string | number {
    switch (this.sortActive) {
      case 'program': return redemption.program ?? '';
      case 'originAirportCode': return redemption.originAirportCode ?? '';
      case 'destinationAirportCode': return redemption.destinationAirportCode ?? '';
      case 'flightDate': return new Date(redemption.flightDate).getTime();
      case 'cabinClass': return redemption.cabinClass;
      case 'passengers': return redemption.passengers;
      case 'milesUsed': return redemption.milesUsed;
      case 'taxes': return redemption.taxes;
      case 'cashPrice': return redemption.cashPrice;
      default: return '';
    }
  }

  private updatePagedData(): void {
    const values = this.data();
    const start = this.pageIndex * this.pageSize;
    this.pagedData.set(values.slice(start, start + this.pageSize));
  }

  openCreateRedemptionDialog(data: RedemptionDialogData = { operation: 'create' }): void {
    const dialogRef = this.dialog.open(CreateRedemptionDialogComponent, { width: '500px', maxWidth: '95vw', data });
    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const request: FlightRedemptionCreateRequest = {
        userId: result.userId,
        programId: result.programId,
        originAirportCode: result.originAirportCode,
        destinationAirportCode: result.destinationAirportCode,
        flightDate: formatDateOnly(result.flightDate),
        cabinClass: Number(result.cabinClass),
        passengers: Number(result.passengers),
        isRoundTrip: result.isRoundTrip,
        milesUsed: Number(result.milesUsed),
        taxes: Number(result.taxes),
        cashPrice: Number(result.cashPrice)
      };
      this.loading.set(true);
      this.redemptionService.createFlightRedemption(request).subscribe({
        next: () => { this.snackbarService.success('Redemption created succesfully.', 3000); this.loadRedemptions(); },
        error: ex => { this.snackbarService.error(`Could not create redemption. ${ex.error.title}`, 3000); this.loading.set(false); }
      });
    });
  }

  openCloneRedemptionDialog(redemption: FlightRedemption): void {
    this.openCreateRedemptionDialog({ operation: 'clone', redemptionId: redemption.id, redemption });
  }

  openEditRedemptionDialog(redemption: FlightRedemption): void {
    const dialogRef = this.dialog.open(CreateRedemptionDialogComponent, {
      width: '700px', data: { operation: 'update', redemptionId: redemption.id, redemption }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const request: FlightRedemptionUpdateRequest = {
        id: redemption.id,
        userId: result.userId,
        programId: result.programId,
        originAirportCode: result.originAirportCode,
        destinationAirportCode: result.destinationAirportCode,
        flightDate: formatDateOnly(result.flightDate),
        cabinClass: Number(result.cabinClass),
        passengers: Number(result.passengers),
        isRoundTrip: result.isRoundTrip,
        milesUsed: Number(result.milesUsed),
        taxes: Number(result.taxes),
        cashPrice: Number(result.cashPrice)
      };
      this.loading.set(true);
      this.redemptionService.updateFlightRedemption(request).subscribe({
        next: () => { this.snackbarService.success('Redemption updated succesfully.', 3000); this.loadRedemptions(); },
        error: ex => { this.snackbarService.error(`Could not update redemption. ${ex.error.title}`, 3000); this.loading.set(false); }
      });
    });
  }

  deleteRedemption(redemption: FlightRedemption): void {
    this.loading.set(true);
    this.redemptionService.deleteFlightRedemption(redemption.id).subscribe({
      next: () => { this.snackbarService.success('Redemption deleted succesfully.', 3000); this.loadRedemptions(); },
      error: () => { this.snackbarService.error('Could not delete redemption.', 3000); this.loading.set(false); }
    });
  }

  getCabinClass(value: number): string {
    switch (value) {
      case 0: return 'Economy';
      case 1: return 'Premium Economy';
      case 2: return 'Business';
      case 3: return 'First';
      default: return 'Unknown';
    }
  }
}
