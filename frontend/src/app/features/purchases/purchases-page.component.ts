  import { Component, inject, OnInit, signal } from '@angular/core';
  import { CommonModule } from '@angular/common';

  import { MatIconModule } from '@angular/material/icon';
  import { MatInputModule } from '@angular/material/input';
  import { MatTableModule } from '@angular/material/table';
  import { MatFormFieldModule } from '@angular/material/form-field';

  import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

  import { PurchasesApiService } from './purchases-api.service';
  import { MileagePurchase, PurchaseCreateRequest, PurchaseUpdateRequest } from './models/mileage-purchase.model';
  import { MatDialog } from '@angular/material/dialog';
  import { CreatePurchaseDialogComponent } from './create-purchase-dialog.component';
  import { TokenStorageService } from '../../core/services/token-storage.service';
  import { PurchaseStatus } from '../../core/enums/purchase-status.enum';

  @Component({
    selector: 'app-purchases-page',
    standalone: true,
    imports: [
      CommonModule,
      MatIconModule,
      MatInputModule,
      MatFormFieldModule,
      MatTableModule,
      AppTableComponent
    ],
    templateUrl: './purchases-page.component.html',
    styleUrls: ['./purchases-page.component.scss']
  })
  export class PurchasesPageComponent implements OnInit {

    data = signal<MileagePurchase[]>([]);
    loading = signal(false);
    isRateLimitReached = signal(false);
    dialog = inject(MatDialog);
    storageService = inject(TokenStorageService);
    readonly PurchaseStatus = PurchaseStatus; 
    
    displayedColumns = [
      'programName',
      'amountPaid',
      'milesPurchased',
      'bonusPercentage',
      'purchaseDate',
      'purchaseStatus',
      'actions'
    ];

    constructor(
      private purchasesService: PurchasesApiService
    ) {}

    ngOnInit(): void {
      this.loadPurchases();
    }

    loadPurchases(): void {

      this.loading.set(true);
      this.isRateLimitReached.set(false);

      this.purchasesService.listPurchases().subscribe({

        next: purchases => {

          console.log('Purchases loaded:', purchases);
          this.data.set(purchases);
          this.loading.set(false);

        },

        error: () => {

          this.loading.set(false);
          this.isRateLimitReached.set(true);

        }

      });

    }

    applyFilter(filter: string): void {

      filter = filter.trim().toLowerCase();

      // TODO: implement filtering

    }

    openEditPurchaseDialog(purchase: MileagePurchase): void {

      const dialogRef = this.dialog.open(CreatePurchaseDialogComponent, {
        width: '600px',
        data: {
          operation: 'update',
          purchaseId: purchase.id
        }
      });

      dialogRef.afterClosed().subscribe(result => {

        if (!result) {
          return;
        }

        const request: PurchaseUpdateRequest = {
          id: purchase.id,
          userId: this.storageService.getUser().id,
          programId: result.programId,
          partner: result.partner,
          description: result.description,
          amountPaid: Number(result.amountPaid),
          milesPurchased: Number(result.milesPurchased),
          bonusPercentage: Number(result.bonusPercentage),
          purchaseStatus: PurchaseStatus.Completed,
          purchaseDate: result.purchaseDate.toISOString().split('T')[0]
        };

        this.purchasesService.updatePurchase(result.id, request).subscribe(() => {
          this.loadPurchases();
        });

      });

    }

    openCreatePurchaseDialog(): void {

      const dialogRef = this.dialog.open(CreatePurchaseDialogComponent, {
        width: '600px',
        data: {
          operation: 'create'
        }
      });

      dialogRef.afterClosed().subscribe(result => {

        if (!result) {
          return;
        }

        const request: PurchaseCreateRequest = {
          userId: this.storageService.getUser().id,
          programId: result.programId,
          partner: result.partner,
          description: result.description,
          amountPaid: Number(result.amountPaid),
          milesPurchased: Number(result.milesPurchased),
          bonusPercentage: Number(result.bonusPercentage),
          purchaseStatus: PurchaseStatus.Completed,
          purchaseDate: result.purchaseDate.toISOString().split('T')[0]
        };

        this.purchasesService.createPurchase(request).subscribe(() => {
          this.loadPurchases();
        });

      });

    }

    getPurchaseStatus(status: PurchaseStatus): string {

      switch (status) {

        case PurchaseStatus.Pending:
          return 'Pending';

        case PurchaseStatus.Completed:
          return 'Completed';

        case PurchaseStatus.Cancelled:
          return 'Cancelled';

        default:
          return 'Unknown';

      }

    }
  }