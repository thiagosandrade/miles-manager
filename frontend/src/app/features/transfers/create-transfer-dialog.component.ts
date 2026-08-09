import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
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
    MatNativeDateModule
  ]
})
export class CreateTransferDialogComponent implements OnInit {

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreateTransferDialogComponent>);
  private programsApi = inject(ProgramsApiService);

  programs: Program[] = [];

  readonly form = this.fb.group({
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

    this.programsApi.listPrograms({
      page: 1,
      pageSize: 1000
    }).then(result => {

      this.programs = result.programs.filter(x => x.isActive);

    });

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