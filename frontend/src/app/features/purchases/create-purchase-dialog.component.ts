import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';
import { MileagePurchase } from './models/mileage-purchase.model';
import { PurchaseStatus } from '../../core/enums/purchase-status.enum';
import { PurchasesApiService } from './purchases-api.service';
import { TranslatePipe } from '@ngx-translate/core';
import { parseDateOnly } from '../../shared/date-utils';
import {
  DateInputComponent,
  MoneyInputComponent,
  NumberInputComponent,
  SelectComponent,
  type SelectOption,
  TextareaInputComponent,
  TextInputComponent
} from '../../shared/components/form-fields';

interface PurchaseDialogData {
  operation: 'create' | 'update' | 'clone';
  purchaseId?: string;
  purchase?: MileagePurchase;
}

@Component({
  selector: 'app-create-purchase-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    TextInputComponent,
    TextareaInputComponent,
    NumberInputComponent,
    MoneyInputComponent,
    DateInputComponent,
    SelectComponent,
    TranslatePipe
  ],
  templateUrl: './create-purchase-dialog.component.html',
  styleUrls: ['./create-purchase-dialog.component.scss']
})
export class CreatePurchaseDialogComponent implements OnInit {

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreatePurchaseDialogComponent>);
  private programsApi = inject(ProgramsApiService);
  private purchasesApi = inject(PurchasesApiService);
  readonly data = inject<PurchaseDialogData>(MAT_DIALOG_DATA);
  programs: Program[] = [];
  programOptions: SelectOption<string>[] = [];

  form = this.fb.group({
    id: [''],
    programId: ['', Validators.required],
    partner: [''],
    description: [''],
    amountPaid: [0, [Validators.required, Validators.min(0)]],
    milesPurchased: [0, [Validators.required, Validators.min(1)]],
    bonusPercentage: [0, [Validators.required, Validators.min(0)]],
    purchaseDate: [new Date(), Validators.required],
    purchaseStatus: [PurchaseStatus.Completed, Validators.required]
  });

  readonly purchaseStatuses = [
    { value: PurchaseStatus.Pending, label: 'Pending' },
    { value: PurchaseStatus.Completed, label: 'Completed' },
    { value: PurchaseStatus.Canceled, label: 'Canceled' }
  ];

  ngOnInit(): void {
    this.programsApi.getPrograms().then(result => {
      this.programs = result.programs.filter(p => p.isActive);
      this.programOptions = this.programs.map(p => ({
        value: p.id,
        label: p.name
      }));

      if ((this.data.operation === 'update' || this.data.operation === 'clone') && this.data.purchaseId) {
        if (this.data.purchase) {
          this.patchPurchase(this.data.purchase);
          return;
        }

        this.purchasesApi.getPurchase(this.data.purchaseId).subscribe({
          next: purchase => this.patchPurchase(purchase)
        });
      }
    });
  }

  readonly purchaseStatusOptions: SelectOption<PurchaseStatus>[] = this.purchaseStatuses.map(status => ({
    value: status.value,
    label: `common.${status.label.toLowerCase()}`
  }));

  private patchPurchase(purchase: MileagePurchase): void {
    this.form.patchValue({
      id: purchase.id,
      programId: this.programs.find(p => p.name === purchase.programName)?.id,
      partner: purchase.partner,
      description: purchase.description,
      amountPaid: purchase.amountPaid,
      milesPurchased: purchase.milesPurchased,
      purchaseStatus: purchase.purchaseStatus,
      purchaseDate: this.data.operation === 'clone' ? new Date() : parseDateOnly(purchase.purchaseDate)
    });
  }

  save(): void {
    if (this.form.invalid) {
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close();
  }

  getPurchaseStatusName(status: PurchaseStatus): string {
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

  getPurchaseStatusClass(status: PurchaseStatus): string {
    switch (status) {
      case PurchaseStatus.Completed:
        return 'success';
      case PurchaseStatus.Pending:
        return 'warning';
      case PurchaseStatus.Canceled:
        return 'danger';
      default:
        return '';
    }
  }

  getSelectedCurrencyCode(): string {
    const programId = this.form.get('programId')?.value;
    const program = this.programs.find(p => p.id === programId);
    return program?.currencyCode ?? 'USD';
  }

  getCurrencySymbol(): string {
    const currencyCode = this.getSelectedCurrencyCode();
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currencyCode,
      currencyDisplay: 'narrowSymbol'
    })
      .formatToParts(0)
      .find(part => part.type === 'currency')
      ?.value ?? currencyCode;
  }
}
