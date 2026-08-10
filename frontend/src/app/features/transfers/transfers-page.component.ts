import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { TransfersApiService } from './transfers-api.service';
import { of, Subject } from 'rxjs';
import { catchError, startWith } from 'rxjs/operators';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MileageTransfer, TransferCreateRequest, TransferUpdateRequest } from './models/mileage-transfer.model';
import { AppTableComponent } from "../../shared/components/app-table/app-table.component";
import { CreateTransferDialogComponent } from './create-transfer-dialog.component';
import { TokenStorageService } from '../../core/services/token-storage.service';

@Component({
  standalone: true,
  selector: 'app-transfers-page',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule,
    MatInputModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    AppTableComponent
],
  templateUrl: './transfers-page.component.html',
  styleUrls: ['./transfers-page.component.scss']
})
export class TransfersPageComponent implements OnInit {
  private api = inject(TransfersApiService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private cd = inject(ChangeDetectorRef);
  private storageService = inject(TokenStorageService);

  displayedColumns = [
    'originProgram',
    'destinationProgram',
    'milesSent',
    'milesReceived',
    'bonusPercentage',
    'transferDate',
    'actions'
  ];

  data: MileageTransfer[] | undefined; 
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;

  filter: string = '';
  searchChanged = new Subject<string>();

  ngOnInit() {
    this.loadTransfers();

    this.searchChanged.pipe(startWith('')).subscribe(() => {
      this.loadTransfers();
    });
  }

  loadTransfers(pageIndex: number = 0, pageSize: number = 10, sortField: string = '', sortDirection: string = '') {
    this.isLoadingResults = true;

    this.api
      .listTransfers({
        page: pageIndex + 1,
        pageSize,
        sort: sortField ? `${sortDirection === 'asc' ? '' : '-'}${sortField}` : undefined,
        filter: this.filter,
      })
      .pipe(
        catchError(() => {
          this.isLoadingResults = false;
          this.isRateLimitReached = true;
          this.snackBar.open('Could not load transfers', 'Close', { duration: 3000 });
          return of(null);
        })
      )
      .subscribe((result) => {
        if (result) {
          this.data = result;
          this.resultsLength = result.length;
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.cd.detectChanges();
        }
      });
  }

  get totalPrograms(): number {

    return new Set(
      (this.data ?? []).flatMap(t => [
        t.originProgram,
        t.destinationProgram
      ])
    ).size;

  }

  applyFilter(filterValue: string) {
    this.filter = filterValue.trim().toLowerCase();
    this.loadTransfers();
  }

  openEditTransferDialog(transfer: MileageTransfer): void {

    const dialogRef = this.dialog.open(CreateTransferDialogComponent, {
      width: '600px',
      data: {
        operation: 'update',
        transferId: transfer.id
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request: TransferUpdateRequest = {
        id: result.id,
        userId: this.storageService.getUser().id,
        originProgramId: result.originProgramId,
        destinationProgramId: result.destinationProgramId,
        milesSent: Number(result.milesSent),
        bonusPercentage: Number(result.bonusPercentage),
        milesReceived: Number(result.milesReceived),
        transferDate: result.transferDate.toISOString().split('T')[0],
        costPerThousandOrigin: Number(result.costPerThousandOrigin),
        costPerThousandDestination: Number(result.costPerThousandDestination)
      };

      this.api.updateTransfer(result.id, request).subscribe(() => {
        this.loadTransfers();
      });

    });

  }

  openCreateTransferDialog(): void {

    const dialogRef = this.dialog.open(CreateTransferDialogComponent, {
      width: '600px',
      data: {
        operation: 'create'
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request: TransferCreateRequest = {
        userId: this.storageService.getUser().id,
        originProgramId: result.originProgramId,
        destinationProgramId: result.destinationProgramId,
        milesSent: Number(result.milesSent),
        bonusPercentage: Number(result.bonusPercentage),
        milesReceived: Number(result.milesReceived),
        transferDate: result.transferDate.toISOString().split('T')[0],
        costPerThousandOrigin: Number(result.costPerThousandOrigin),
        costPerThousandDestination: Number(result.costPerThousandDestination)
      };

      this.api.createTransfer(request).subscribe(() => {
        this.loadTransfers();
      });

    });

  }
}
