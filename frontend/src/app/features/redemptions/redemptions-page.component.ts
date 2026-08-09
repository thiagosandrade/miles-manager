import { Component, inject, OnInit, signal } from '@angular/core';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { FlightRedemptionService } from './flight-redemption.service';
import { FlightRedemption, FlightRedemptionCreateRequest } from './models/flight-redemption.model';
import { MatIconModule } from "@angular/material/icon";
import { MatTableModule } from '@angular/material/table';
import { CreateRedemptionDialogComponent } from './create-redemption-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-redemptions-page',
  standalone: true,
  imports: [CommonModule, MatCardModule, AppTableComponent, MatIconModule, MatTableModule],
  templateUrl: './redemptions-page.component.html',
  styleUrls: ['./redemptions-page.component.scss']
})
export class RedemptionsPageComponent implements OnInit {
  private dialog = inject(MatDialog);

  redemptions = signal<FlightRedemption[]>([]);
  loading = signal<boolean>(false);
  
  displayedColumns = [
    'program',
    'originAirport',
    'destinationAirport',
    'flightDate',
    'cabinClass',
    'passengers',
    'milesUsed',
    'taxes',
    'cashPrice',
    'effectiveCostPerThousand'
  ];
  
  constructor(private redemptionService: FlightRedemptionService) {}

  ngOnInit(): void {
    this.loadRedemptions();
  }

  loadRedemptions(): void {

    this.loading.set(true);

    this.redemptionService.getFlightRedemptions().subscribe({

      next: data => {

        this.redemptions.set(data);
        this.loading.set(false);

      },

      error: () => {

        this.loading.set(false);

      }

    });

  }

  openCreateRedemptionDialog(): void {

    const dialogRef = this.dialog.open(CreateRedemptionDialogComponent, {
      width: '700px'
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request: FlightRedemptionCreateRequest = {
        userId: result.userId,
        programId: result.programId,
        OriginAirport: result.originAirportCode,
        DestinationAirport: result.destinationAirportCode,
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

        next: () => this.loadRedemptions(),

        error: () => this.loading.set(false)

      });

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
