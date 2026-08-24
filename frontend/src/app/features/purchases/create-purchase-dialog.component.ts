import { CommonModule } from "@angular/common";
import { Component, OnInit, inject } from "@angular/core";
import { ReactiveFormsModule, FormBuilder, Validators } from "@angular/forms";

import { MatButtonModule } from "@angular/material/button";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatSelectModule } from "@angular/material/select";
import { ProgramsApiService } from "../programs/programs-api.service";
import { Program } from "../programs/models/program.model";
import { PurchaseStatus } from "../../core/enums/purchase-status.enum";
import { PurchasesApiService } from "./purchases-api.service";
import { TranslatePipe } from "@ngx-translate/core";

interface PurchaseDialogData {
  operation: 'create' | 'update';
  purchaseId?: string;
}

@Component({
  selector: 'app-create-purchase-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
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

  purchaseStatuses = [
    { value: PurchaseStatus.Pending, label: 'Pending' },
    { value: PurchaseStatus.Completed, label: 'Completed' },
    { value: PurchaseStatus.Canceled, label: 'Canceled' }
  ];

  ngOnInit(): void {

    this.programsApi.listPrograms({
      page: 1,
      pageSize: 1000
    }).then(result => {

      this.programs = result.programs.filter(p => p.isActive);

      if (this.data.operation === 'update' && this.data.purchaseId) {

        this.purchasesApi.getPurchase(this.data.purchaseId).subscribe({

          next: purchase => {

            this.form.patchValue({
              id: purchase.id,
              programId: this.programs.find(p => p.name === purchase.programName)?.id,
              partner: purchase.partner,
              description: purchase.description,
              amountPaid: purchase.amountPaid,
              milesPurchased: purchase.milesPurchased,
              bonusPercentage: purchase.bonusPercentage,
              purchaseStatus: purchase.purchaseStatus,
              purchaseDate: new Date(purchase.purchaseDate)
            });

          }

        });

      }

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

    const program = this.programs.find(
      p => p.id === programId
    );

    return program?.currencyCode ?? 'USD';
  }

  getCurrencySymbol(): string {
    const currencyCode = this.getSelectedCurrencyCode();

    return new Intl.NumberFormat(
      'en-US',
      {
        style: 'currency',
        currency: currencyCode,
        currencyDisplay: 'narrowSymbol'
      }
    )
      .formatToParts(0)
      .find(part => part.type === 'currency')
      ?.value ?? currencyCode;
  }
}