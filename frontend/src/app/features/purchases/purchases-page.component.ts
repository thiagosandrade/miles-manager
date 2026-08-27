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
import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';

  @Component({
    selector: 'app-purchases-page',
    standalone: true,
    imports: [
      CommonModule,
      MatIconModule,
      MatInputModule,
      MatFormFieldModule,
      MatTableModule,
      AppTableComponent,
      TranslatePipe
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
    snackbarService = inject(SnackbarService);

    readonly PurchaseStatus = PurchaseStatus; 
    
    displayedColumns = [
      'programName',
      'amountPaid',
      'milesPurchased',
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
          this.data.set(purchases);
          this.loading.set(false);
        },

        error: () => {

          this.loading.set(false);
          this.isRateLimitReached.set(true);

        }

      });

    }

    get averagePurchase(): number {
      if (!this.data().length) {
        return 0;
      }

      return this.data().reduce((sum, p) => sum + p.amountPaid, 0) / this.data().length;
    }

    applyFilter(filter: string): void {

      filter = filter.trim().toLowerCase();

      // TODO: implement filtering

    }

    openEditPurchaseDialog(purchase: MileagePurchase): void {

      const dialogRef = this.dialog.open(CreatePurchaseDialogComponent, {
        width: '500px',
        maxWidth: '95vw',
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
          bonusPercentage: purchase.bonusPercentage,
          purchaseStatus: PurchaseStatus.Completed,
          purchaseDate: result.purchaseDate.toISOString().split('T')[0]
        };

        this.loading.set(true);

        this.purchasesService.updatePurchase(result.id, request).subscribe({
          next: () => {
            this.snackbarService.success('Purchase updated succesfully.', 3000);
            this.loadPurchases();
            this.loading.set(false)
          },
          error: () => {
            this.snackbarService.error('Could not create purchase.', 3000);
            this.loading.set(false)
          } 
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
          bonusPercentage: 0,
          purchaseStatus: PurchaseStatus.Completed,
          purchaseDate: result.purchaseDate.toISOString().split('T')[0]
        };

        this.purchasesService.createPurchase(request).subscribe({
          next: () => {
            this.snackbarService.success('Purchase created succesfully.', 3000);
            this.loadPurchases();
            this.loading.set(false)
          },
          error: () => {
            this.snackbarService.error('Could not create purchase.', 3000);
            this.loading.set(false)
          } 
        });

      });

    }

    clonePurchase(purchase: MileagePurchase): void {

      const dialogRef = this.dialog.open(CreatePurchaseDialogComponent, {
        width: '600px',
        maxWidth: '95vw',
        data: {
          operation: 'clone',
          purchaseId: purchase.id
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
          bonusPercentage: 0,
          purchaseStatus: PurchaseStatus.Completed,
          purchaseDate: result.purchaseDate.toISOString().split('T')[0]
        };

        this.purchasesService.createPurchase(request).subscribe({
          next: () => {
            this.snackbarService.success('Purchase created succesfully.', 3000);
            this.loadPurchases();
            this.loading.set(false)
          },
          error: () => {
            this.snackbarService.error('Could not create purchase.', 3000);
            this.loading.set(false)
          } 
        });

      });

    }

    getPurchaseStatus(status: PurchaseStatus): string {

      switch (status) {

        case PurchaseStatus.Pending:
          return 'Pending';

        case PurchaseStatus.Completed:
          return 'Completed';

        case PurchaseStatus.Canceled:
          return 'Canceled';

        default:
          return 'Unknown';

      }

    }

    deletePurchase(purchase: MileagePurchase): void {
      
      this.purchasesService.deletePurchase(purchase.id).subscribe({
        next: () => {
          this.snackbarService.success('Purchase deleted succesfully.', 3000);
          this.loadPurchases();
          this.loading.set(false)
        },
        error: () => {
          this.snackbarService.error('Could not delete purchase.', 3000);
          this.loading.set(false)
        } 
      });
    }

  }