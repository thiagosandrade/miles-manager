import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';

import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';
import { TransfersApiService } from './transfers-api.service';
import { TranslatePipe } from '@ngx-translate/core';

interface TransferDialogData {
  operation: 'create' | 'update';
  transferId?: string;
}

@Component({
  selector: 'app-create-transfer-dialog',
  standalone: true,
  templateUrl: './create-transfer-dialog.component.html',
  styleUrls: ['./create-transfer-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    TranslatePipe
  ]
})
export class CreateTransferDialogComponent implements OnInit {

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreateTransferDialogComponent>);
  private programsApi = inject(ProgramsApiService);
  private transfersApi = inject(TransfersApiService);

  data = inject<TransferDialogData>(MAT_DIALOG_DATA);

  programs: Program[] = [];

  readonly form = this.fb.group({
    id: [''],
    originProgramId: ['', Validators.required],
    destinationProgramId: ['', Validators.required],

    milesSent: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    bonusPercentage: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    milesReceived: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    transferDate: [
      new Date(),
      Validators.required
    ],

    costPerThousandOrigin: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    costPerThousandDestination: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],
  });

  ngOnInit(): void {
    this.programsApi.getPrograms({
      page: 1,
      pageSize: 1000
    }).then(result => {

      this.programs = result.programs.filter(x => x.isActive);

    });

    if (this.data?.operation === 'update' && this.data?.transferId) {
        this.transfersApi.getTransfer(this.data.transferId).subscribe({
            next: transfer => {
                this.form.patchValue({
                    id: transfer.id,
                    originProgramId: this.programs.find(p => p.name === transfer.originProgram)?.id,
                    destinationProgramId: this.programs.find(p => p.name === transfer.destinationProgram)?.id ,
                    milesSent: transfer.milesSent,
                    bonusPercentage: transfer.bonusPercentage,
                    milesReceived: transfer.milesReceived,
                    transferDate: new Date(transfer.transferDate),
                    costPerThousandOrigin: transfer.costPerThousandOrigin,
                    costPerThousandDestination: transfer.costPerThousandDestination
                });
            }
        });
    }
    else {
        this.form.reset();
    }
  }

  save(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();
      return;

    }

    this.dialogRef.close(this.form.getRawValue());

  }

  cancel(): void {

    this.dialogRef.close();

  }

}