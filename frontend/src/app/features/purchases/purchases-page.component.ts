  import { Component, inject, OnInit, signal } from '@angular/core';
  import { CommonModule } from '@angular/common';

  import { MatIconModule } from '@angular/material/icon';
  import { MatInputModule } from '@angular/material/input';
  import { MatTableModule } from '@angular/material/table';
  import { MatFormFieldModule } from '@angular/material/form-field';
  import { PageEvent } from '@angular/material/paginator';

  import { AppTableComponent } from '../../shared/components/app-table/app-table.component';

  import { PurchasesApiService } from './purchases-api.service';
  import { MileagePurchase, PurchaseCreateRequest, PurchaseUpdateRequest } from './models/mileage-purchase.model';
  import { MatDialog } from '@angular/material/dialog';
  import { CreatePurchaseDialogComponent } from './create-purchase-dialog.component';
  import { TokenStorageService } from '../../core/services/token-storage.service';
  import { PurchaseStatus } from '../../core/enums/purchase-status.enum';
  import { SnackbarService } from '../../core/services/snackbar.service';
import { TranslatePipe } from '@ngx-translate/core';
import { formatDateOnly } from '../../shared/date-utils';

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

    readonly allData = signal<MileagePurchase[]>([]);
    readonly data = signal<MileagePurchase[]>([]);

    pagedData = signal<MileagePurchase[]>([]);
    private pageIndex = 0;
    private pageSize = 5;
    private currentFilter = '';
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
          this.allData.set(purchases);
          this.applyFilter(this.currentFilter);
          this.loading.set(false);
        },

        error: () => {

          this.loading.set(false);
          this.isRateLimitReached.set(true);

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

    get averagePurchase(): number {
      if (!this.data().length) {
        return 0;
      }

      return this.data().reduce((sum, p) => sum + p.amountPaid, 0) / this.data().length;
    }

    applyFilter(filter: string): void {

      this.currentFilter = filter;
      const normalizedFilter = filter.trim().toLowerCase();

      const filtered = this.allData().filter(item =>
        (item.programName?.toLowerCase().includes(normalizedFilter)
        || item.description?.toLowerCase().includes(normalizedFilter)
        || item.partner?.toLowerCase().includes(normalizedFilter)) ?? false
      );

      this.data.set(filtered);
      this.updatePagedData();

    }

    openEditPurchaseDialog(purchase: MileagePurchase): void {

      const dialogRef = this.dialog.open(CreatePurchaseDialogComponent, {
        width: '500px',
        maxWidth: '95vw',
        data: {
          operation: 'update',
          purchaseId: purchase.id,
          purchase
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
          purchaseStatus: result.purchaseStatus,
          purchaseDate: formatDateOnly(result.purchaseDate)
        };

        this.loading.set(true);

        this.purchasesService.updatePurchase(result.id, request).subscribe({
          next: () => {
            this.snackbarService.success('Purchase updated succesfully.', 3000);
            this.loadPurchases();
            this.loading.set(false)
          },
          error: (ex) => {
            this.snackbarService.error(`Could not create purchase. ${ex.error.title}`, 3000);
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
          purchaseStatus: result.purchaseStatus,
          purchaseDate: formatDateOnly(result.purchaseDate)
        };

        this.purchasesService.createPurchase(request).subscribe({
          next: () => {
            this.snackbarService.success('Purchase created succesfully.', 3000);
            this.loadPurchases();
            this.loading.set(false)
          },
          error: (ex) => {
            this.snackbarService.error(`Could not create purchase. ${ex.error.title}`, 3000);
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
          purchaseId: purchase.id,
          purchase
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
          purchaseStatus: result.purchaseStatus,
          purchaseDate: formatDateOnly(result.purchaseDate)
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
