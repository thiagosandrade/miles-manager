import { Component, inject, OnInit, signal } from '@angular/core';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { FlightRedemptionService } from './flight-redemption.service';
import { FlightRedemption, FlightRedemptionCreateRequest, FlightRedemptionUpdateRequest } from './models/flight-redemption.model';
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from '@angular/material/table';
import { PageEvent } from '@angular/material/paginator';
import { CreateRedemptionDialogComponent, RedemptionDialogData } from './create-redemption-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-redemptions-page',
  standalone: true,
  imports: [
    CommonModule, 
    MatCardModule, 
    MatInputModule,
    AppTableComponent, 
    MatIconModule, 
    MatTableModule,
    TranslatePipe
  ],
  templateUrl: './redemptions-page.component.html',
  styleUrls: ['./redemptions-page.component.scss']
})
export class RedemptionsPageComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  readonly allData = signal<FlightRedemption[]>([]);
  readonly data = signal<FlightRedemption[]>([]);
  
  pagedData = signal<FlightRedemption[]>([]);
  private pageIndex = 0;
  private pageSize = 5;
  loading = signal<boolean>(false);
  
  displayedColumns = [
    'program',
    'originAirportCode',
    'destinationAirportCode',
    'flightDate',
    'cabinClass',
    'passengers',
    'milesUsed',
    'taxes',
    'cashPrice',
    'effectiveCostPerThousand',
    'actions'
  ];
  
  constructor(private redemptionService: FlightRedemptionService) {}

  ngOnInit(): void {
    this.loadRedemptions();
  }

  loadRedemptions(): void {

    this.loading.set(true);

    this.redemptionService.getFlightRedemptions().subscribe({

      next: data => {

        this.allData.set(data);
        this.data.set(data);
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
      item.program?.toLowerCase().includes(normalizedFilter) ?? false
    );

    this.data.set(filtered);
    this.updatePagedData();

  }

  openCreateRedemptionDialog(data: RedemptionDialogData = { operation: 'create' }): void {

    const dialogRef = this.dialog.open(CreateRedemptionDialogComponent, {
       width: '500px',
      maxWidth: '95vw',
      data
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request: FlightRedemptionCreateRequest = {
        userId: result.userId,
        programId: result.programId,
        originAirportCode: result.originAirportCode,
        destinationAirportCode: result.destinationAirportCode,
        flightDate: result.flightDate.toISOString().split('T')[0],
        cabinClass: Number(result.cabinClass),
        passengers: Number(result.passengers),
        isRoundTrip: result.isRoundTrip,
        milesUsed: Number(result.milesUsed),
        taxes: Number(result.taxes),
        cashPrice: Number(result.cashPrice)
      };

      this.loading.set(true);

      this.redemptionService.createFlightRedemption(request).subscribe({
          next: () => {
            this.snackbarService.success('Redemption updated succesfully.', 3000);
            this.loadRedemptions();
            this.loading.set(false)
          },
          error: () => {
            this.snackbarService.error('Could not create redemption.', 3000);
            this.loading.set(false)
          } 
        });

    });

  }

  openCloneRedemptionDialog(redemption: FlightRedemption): void {
    this.openCreateRedemptionDialog({
      operation: 'clone',
      redemptionId: redemption.id,
      redemption
    });
  }

  openEditRedemptionDialog(redemption: FlightRedemption): void {

    const dialogRef = this.dialog.open(CreateRedemptionDialogComponent, {
      width: '700px',
      data: {
        operation: 'update',
        redemptionId: redemption.id,
        redemption
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request : FlightRedemptionUpdateRequest = {
        id: redemption.id,
        userId: result.userId,
        programId: result.programId,
        originAirportCode: result.originAirportCode,
        destinationAirportCode: result.destinationAirportCode,
        flightDate: result.flightDate.toISOString().split('T')[0],
        cabinClass: Number(result.cabinClass),
        passengers: Number(result.passengers),
        isRoundTrip: result.isRoundTrip,
        milesUsed: Number(result.milesUsed),
        taxes: Number(result.taxes),
        cashPrice: Number(result.cashPrice)
      };

      this.loading.set(true);

      this.redemptionService.updateFlightRedemption(request).subscribe({

        next: () => {
          this.snackbarService.success('Redemption updated succesfully.', 3000);
          this.loadRedemptions();
          this.loading.set(false);
        },
        error: () => {
          this.snackbarService.error('Could not update redemption.', 3000);
          this.loading.set(false);
        } 
      });

    });

  }

  deleteRedemption(redemption: FlightRedemption): void {
    
    this.loading.set(true);

    this.redemptionService.deleteFlightRedemption(redemption.id).subscribe({

      next: () => {
        this.snackbarService.success('Redemption deleted succesfully.', 3000);
        this.loadRedemptions();
        this.loading.set(false);
      },

      error: () => {
        this.snackbarService.error('Could not delete redemption.', 3000);
        this.loading.set(false);
      }

    });
  }

  getCabinClass(value: number): string {

    switch (value) {

      case 0:
        return 'Economy';

      case 1:
        return 'Premium Economy';

      case 2:
        return 'Business';

      case 3:
        return 'First';

      default:
        return 'Unknown';

    }

  }
}
