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
import { Transfer, TransferCreateRequest } from './models/mileage-transfer.model';
import { AppTableComponent } from "../../shared/components/app-table/app-table.component";
import { CreateTransferDialogComponent } from './create-transfer-dialog.component';

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

  displayedColumns: string[] = ['fromProgramId', 'toProgramId', 'amount', 'transferDate', 'actions'];
  data: Transfer[] | undefined; 
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
          this.data = result.transfers;
          this.resultsLength = result.totalCount;
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.cd.detectChanges();
        }
      });
  }

  get totalPrograms(): number {
    const transfers = this.data ?? [];

    return new Set(
      transfers.flatMap(t => [t.fromProgramId, t.toProgramId])
    ).size;
  }

  applyFilter(filterValue: string) {
    this.filter = filterValue.trim().toLowerCase();
    this.loadTransfers();
  }

  openCreateTransferDialog(): void {

    const dialogRef = this.dialog.open(CreateTransferDialogComponent, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result) {
        return;
      }

      const request: TransferCreateRequest = {
        fromProgramId: result.fromProgramId,
        toProgramId: result.toProgramId,
        amount: Number(result.amount),
        transferDate: result.transferDate
      };

      this.api.createTransfer(request).subscribe(() => {
        this.loadTransfers();
      });

    });

  }
}
