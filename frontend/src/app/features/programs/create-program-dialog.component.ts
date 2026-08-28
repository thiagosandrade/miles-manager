import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';

import {
  MatDialogRef,
  MatDialogModule,
  MAT_DIALOG_DATA
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { ProgramsApiService } from './programs-api.service';
import { ProgramCreateRequest, ProgramUpdateRequest } from './models/program.model';
import { TranslatePipe } from '@ngx-translate/core';
import {
  TextInputComponent,
  NumberInputComponent,
  SelectComponent,
  CheckboxComponent
} from '../../shared/components/form-fields/form-fields';

interface ProgramDialogData {
  operation: 'create' | 'update';
  programId?: string;
}

@Component({
  selector: 'app-create-program-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    TranslatePipe,
    TextInputComponent,
    NumberInputComponent,
    SelectComponent,
    CheckboxComponent
  ],
  templateUrl: './create-program-dialog.component.html',
  styleUrls: ['./create-program-dialog.component.scss']
})
export class CreateProgramDialogComponent implements OnInit {

  readonly fb = inject(FormBuilder);
  readonly api = inject(ProgramsApiService);
  readonly dialogRef = inject(MatDialogRef<CreateProgramDialogComponent>);
  readonly data = inject<ProgramDialogData>(MAT_DIALOG_DATA);

  loading = false;

  readonly programTypeOptions = [
    { value: 0, viewValue: 'Airline' },
    { value: 1, viewValue: 'Hotel' },
    { value: 2, viewValue: 'Bank' },
    { value: 3, viewValue: 'Credit Card' }
  ];

  readonly currencyOptions = [
    { value: 'BRL', viewValue: 'BRL' },
    { value: 'EUR', viewValue: 'EUR' },
    { value: 'USD', viewValue: 'USD' }
  ];

  readonly form = this.fb.group({
    id: [''],
    programCode: ['', Validators.required],
    name: ['', Validators.required],
    programType: [0, Validators.required],
    website: [''],
    logo: [''],
    referencePricePerThousand: [0, [Validators.required, Validators.min(0)]],
    isActive: [true],
    currencyCode: ['EUR', Validators.required]
  });

  ngOnInit(): void {
    if (this.data?.operation === 'update' && this.data?.programId) {
      this.api.getProgram(this.data.programId).subscribe({
        next: program => this.form.patchValue(program)
      });
    }
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const value = this.form.getRawValue();

    if (this.data?.operation === 'update' && this.data?.programId) {
      const request: ProgramUpdateRequest = {
        id: this.data.programId,
        programCode: value.programCode!,
        name: value.name!,
        programType: Number(value.programType),
        website: value.website ?? null,
        logo: value.logo ?? null,
        referencePricePerThousand: Number(value.referencePricePerThousand),
        isActive: value.isActive ?? true,
        currencyCode: value.currencyCode!
      };

      this.api.updateProgram(this.data.programId, request).subscribe({
        next: () => {
          this.loading = false;
          this.dialogRef.close(true);
        },
        error: () => this.loading = false
      });
      return;
    }

    const request: ProgramCreateRequest = {
      programCode: value.programCode!,
      name: value.name!,
      programType: Number(value.programType),
      website: value.website ?? null,
      logo: value.logo ?? null,
      referencePricePerThousand: Number(value.referencePricePerThousand),
      isActive: value.isActive ?? true,
      currencyCode: value.currencyCode!
    };

    this.api.createProgram(request).subscribe({
      next: () => {
        this.loading = false;
        this.dialogRef.close(true);
      },
      error: () => this.loading = false
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}