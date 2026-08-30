import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';

import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';
import {
  NumberInputComponent,
  SelectComponent,
  type SelectOption
} from '../../shared/components/form-fields/form-fields';
import { ProgramFactorsApiService } from './program-factors-api.service';
import { ProgramFactor } from './models/program-factor.model';

export interface ProgramFactorDialogData {
  operation: 'create' | 'update';
  programFactor?: ProgramFactor;
}

@Component({
  selector: 'app-create-program-factor-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    TranslatePipe,
    SelectComponent,
    NumberInputComponent
  ],
  templateUrl: './create-program-factor-dialog.component.html',
  styleUrls: ['./create-program-factor-dialog.component.scss']
})
export class CreateProgramFactorDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<CreateProgramFactorDialogComponent>);
  private readonly programsApi = inject(ProgramsApiService);
  private readonly factorsApi = inject(ProgramFactorsApiService);

  readonly data = inject<ProgramFactorDialogData>(MAT_DIALOG_DATA);

  programs: Program[] = [];
  programOptions: SelectOption<string>[] = [];
  loading = false;

  readonly form = this.fb.group({
    originProgramId: ['', Validators.required],
    destinationProgramId: ['', Validators.required],
    fromAmount: [1, [Validators.required, Validators.min(0.0001)]],
    toAmount: [1, [Validators.required, Validators.min(0.0001)]]
  });

  ngOnInit(): void {
    this.programsApi.getPrograms().then(result => {
      this.programs = result.programs.filter(program => program.isActive);
      this.programOptions = this.programs.map(program => ({
        value: program.id,
        label: program.name
      }));

      if (this.data.operation === 'update' && this.data.programFactor) {
        this.form.patchValue(this.data.programFactor);
      }
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    if (value.originProgramId === value.destinationProgramId) {
      this.form.controls.destinationProgramId.setErrors({ sameProgram: true });
      return;
    }

    this.loading = true;

    const request = {
      originProgramId: value.originProgramId!,
      destinationProgramId: value.destinationProgramId!,
      fromAmount: Number(value.fromAmount),
      toAmount: Number(value.toAmount)
    };

    const request$ = this.data.operation === 'update' && this.data.programFactor
      ? this.factorsApi.update(this.data.programFactor.id, request)
      : this.factorsApi.create(request);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.dialogRef.close(true);
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
