import { Component, inject } from '@angular/core';
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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { Program } from '../programs/models/program.model';
import { ProgramsApiService } from '../programs/programs-api.service';

@Component({
  selector: 'app-create-redemption-dialog',
  standalone: true,
  templateUrl: './create-redemption-dialog.component.html',
  styleUrls: ['./create-redemption-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatCheckboxModule
  ]
})
export class CreateRedemptionDialogComponent {

  private fb = inject(FormBuilder);
  private programsApi = inject(ProgramsApiService);

  private dialogRef =
    inject(MatDialogRef<CreateRedemptionDialogComponent>);

  programs: Program[] = [];

  readonly form = this.fb.group({

    programId: ['', Validators.required],

    OriginAirport: ['', Validators.required],

    DestinationAirport: ['', Validators.required],

    flightDate: [
      new Date(),
      Validators.required
    ],

    cabinClass: [
      0,
      Validators.required
    ],

    passengers: [
      1,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    isRoundTrip: false,

    milesUsed: [
      0,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    taxes: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    cashPrice: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ]

  });

  ngOnInit(): void {

    this.programsApi.listPrograms({
      page: 1,
      pageSize: 1000
    }).then(result => {
      this.programs = result.programs.filter(p => p.isActive);
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