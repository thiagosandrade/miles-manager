import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatInputModule } from '@angular/material/input';
import { PageEvent } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { TranslatePipe } from '@ngx-translate/core';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { TransfersApiService } from './transfers-api.service';
import { MileageTransfer, TransferCreateRequest, TransferUpdateRequest } from './models/mileage-transfer.model';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { CreateTransferDialogComponent, TransferDialogData } from './create-transfer-dialog.component';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { SnackbarService } from '../../core/services/snackbar.service';

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
    AppTableComponent,
    TranslatePipe
  ],
  templateUrl: './transfers-page.component.html',
  styleUrls: ['./transfers-page.component.scss']
})
export class TransfersPageComponent implements OnInit {
  private api = inject(TransfersApiService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private storageService = inject(TokenStorageService);

  displayedColumns = ['originProgram', 'destinationProgram', 'milesSent', 'milesReceived', 'bonusPercentage', 'transferDate', 'actions'];
  readonly allData = signal<MileageTransfer[]>([]);
  readonly data = signal<MileageTransfer[]>([]);
  readonly loading = signal(false);
  readonly isRateLimitReached = signal(false);
  pagedData: MileageTransfer[] = [];

  private pageIndex = 0;
  private pageSize = 10;
  private sortActive = '';
  private sortDirection: 'asc' | 'desc' | '' = '';
  filter = '';

  ngOnInit(): void {
    this.loadTransfers();
  }

  loadTransfers(): void {
    this.loading.set(true);
    this.isRateLimitReached.set(false);

    this.api.listTransfers().pipe(
      catchError(() => {
        this.loading.set(false);
        this.isRateLimitReached.set(true);
        this.snackbarService.error('Could not load transfers', 3000);
        return of(null);
      })
    ).subscribe(result => {
      if (!result) return;
      this.allData.set(result);
      this.applyFilter(this.filter, false);
      this.loading.set(false);
    });
  }

  onSortChange(event: Sort): void {
    this.sortActive = event.direction ? event.active : '';
    this.sortDirection = event.direction;
    this.pageIndex = 0;
    this.applyFilter(this.filter, false);
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePagedData();
  }

  get totalPrograms(): number {
    return new Set(this.data().flatMap(transfer => [transfer.originProgram, transfer.destinationProgram])).size;
  }

  applyFilter(filter: string, _reload = true): void {
    this.filter = filter;
    const normalizedFilter = filter.trim().toLowerCase();
    const filtered = this.allData().filter(item =>
      [item.originProgram, item.destinationProgram]
        .some(value => value?.toLowerCase().includes(normalizedFilter))
    );

    this.data.set(this.sortData(filtered));
    this.updatePagedData();
  }

  private sortData(values: MileageTransfer[]): MileageTransfer[] {
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

  private getSortValue(transfer: MileageTransfer): string | number {
    switch (this.sortActive) {
      case 'originProgram': return transfer.originProgram ?? '';
      case 'destinationProgram': return transfer.destinationProgram ?? '';
      case 'milesSent': return transfer.milesSent ?? 0;
      case 'milesReceived': return transfer.milesReceived ?? 0;
      case 'bonusPercentage': return transfer.bonusPercentage ?? 0;
      case 'transferDate': return new Date(transfer.transferDate).getTime();
      default: return '';
    }
  }

  private updatePagedData(): void {
    const values = this.data();
    const start = this.pageIndex * this.pageSize;
    this.pagedData = values.slice(start, start + this.pageSize);
  }

  openEditTransferDialog(transfer: MileageTransfer): void {
    const dialogRef = this.dialog.open(CreateTransferDialogComponent, {
      width: '500px',
      maxWidth: '95vw',
      data: { operation: 'update', transferId: transfer.id }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const request: TransferUpdateRequest = {
        id: result.id,
        userId: this.storageService.getUser().id,
        originProgramId: result.originProgramId,
        destinationProgramId: result.destinationProgramId,
        milesSent: Number(result.milesSent),
        bonusPercentage: Number(result.bonusPercentage),
        milesReceived: Number(result.milesReceived),
        factorFrom: Number(result.factorFrom),
        factorTo: Number(result.factorTo),
        transferDate: result.transferDate.toISOString().split('T')[0]
      };

      this.api.updateTransfer(result.id, request).subscribe({
        next: () => { this.snackbarService.success('Transfer updated succesfully.', 3000); this.loadTransfers(); },
        error: () => { this.snackbarService.error('Could not update transfer.', 3000); }
      });
    });
  }

  openCloneTransferDialog(transfer: MileageTransfer): void {
    this.openCreateTransferDialog({ operation: 'clone', transferId: transfer.id, transfer });
  }

  openCreateTransferDialog(data: TransferDialogData = { operation: 'create' }): void {
    const dialogRef = this.dialog.open(CreateTransferDialogComponent, { width: '600px', data });
    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const request: TransferCreateRequest = {
        userId: this.storageService.getUser().id,
        originProgramId: result.originProgramId,
        destinationProgramId: result.destinationProgramId,
        milesSent: Number(result.milesSent),
        bonusPercentage: Number(result.bonusPercentage),
        milesReceived: Number(result.milesReceived),
        factorFrom: Number(result.factorFrom),
        factorTo: Number(result.factorTo),
        transferDate: result.transferDate.toISOString().split('T')[0]
      };

      this.api.createTransfer(request).subscribe({
        next: () => { this.snackbarService.success('Transfer created succesfully.', 3000); this.loadTransfers(); },
        error: ex => { this.snackbarService.error(`Could not create transfer. ${ex.error.title}`, 3000); }
      });
    });
  }

  deleteTransfer(transfer: MileageTransfer): void {
    this.api.deleteTransfer(transfer.id).subscribe({
      next: () => { this.snackbarService.success('Transfer deleted succesfully.', 3000); this.loadTransfers(); },
      error: ex => { this.snackbarService.error(`Could not delete transfer. ${ex.error.title}`, 3000); }
    });
  }
}
