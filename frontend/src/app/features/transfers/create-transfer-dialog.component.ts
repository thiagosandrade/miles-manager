import { Component, DestroyRef, OnInit, inject } from '@angular/core';
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

import { MatButtonModule } from '@angular/material/button';

import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';
import { TransfersApiService } from './transfers-api.service';
import { TranslatePipe } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  DateInputComponent,
  NumberInputComponent,
  SelectComponent,
  type SelectOption
} from '../../shared/components/form-fields/form-fields';
import { MileageTransfer } from './models/mileage-transfer.model';
import { ProgramFactorsApiService } from '../program-factors/program-factors-api.service';
import { ProgramFactor } from '../program-factors/models/program-factor.model';

export interface TransferDialogData {
  operation: 'create' | 'update' | 'clone';
  transferId?: string;
  transfer?: MileageTransfer;
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
    MatButtonModule,
    SelectComponent,
    NumberInputComponent,
    DateInputComponent,
    TranslatePipe
  ]
})
export class CreateTransferDialogComponent implements OnInit {

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CreateTransferDialogComponent>);
  private programsApi = inject(ProgramsApiService);
  private transfersApi = inject(TransfersApiService);
  private factorsApi = inject(ProgramFactorsApiService);
  private destroyRef = inject(DestroyRef);

  data = inject<TransferDialogData>(MAT_DIALOG_DATA);

  programs: Program[] = [];
  programOptions: SelectOption<string>[] = [];
  originProgramOptions: SelectOption<string>[] = [];
  destinationProgramOptions: SelectOption<string>[] = [];
  programFactors: ProgramFactor[] = [];
  selectedFactor: ProgramFactor | null = null;

  readonly form = this.fb.group({
    id: [''],
    originProgramId: ['', Validators.required],
    destinationProgramId: ['', Validators.required],
    milesSent: [0, [Validators.required, Validators.min(1)]],
    bonusPercentage: [0, [Validators.required, Validators.min(0)]],
    milesReceived: [0, [Validators.required, Validators.min(1)]],
    transferDate: [new Date(), Validators.required],
  });

  ngOnInit(): void {
    this.form.controls.milesSent.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateMilesReceived());

    this.form.controls.bonusPercentage.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.updateMilesReceived());

    this.form.controls.originProgramId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.onProgramSelectionChanged());

    this.form.controls.destinationProgramId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.onProgramSelectionChanged());

    this.programsApi.getPrograms().then(result => {
      this.programs = result.programs.filter(x => x.isActive);
      this.programOptions = this.programs.map(program => ({
        value: program.id,
        label: program.name
      }));
      this.updateProgramOptions();

      this.factorsApi.getAll().then(factors => {
        this.programFactors = factors;
        this.updateSelectedFactor();
      });

      if ((this.data?.operation === 'update' || this.data.operation === 'clone') && this.data?.transferId) {
        this.transfersApi.getTransfer(this.data.transferId).subscribe({
          next: transfer => {
            this.form.patchValue({
              id: this.data.operation === 'clone' ? '' : transfer.id,
              originProgramId: this.programs.find(p => p.name === transfer.originProgram)?.id,
              destinationProgramId: this.programs.find(p => p.name === transfer.destinationProgram)?.id,
              milesSent: transfer.milesSent,
              bonusPercentage: transfer.bonusPercentage,
              transferDate: new Date(transfer.transferDate)
            }, { emitEvent: false });
            this.updateProgramOptions();
            this.updateSelectedFactor();
            this.updateMilesReceived();
          }
        });
      } else {
        this.updateMilesReceived();
      }
    });
  }

  private onProgramSelectionChanged(): void {
    this.updateProgramOptions();
    this.updateSelectedFactor();
    this.updateMilesReceived();
  }

  private updateProgramOptions(): void {
    const originId = this.form.controls.originProgramId.value;
    const destinationId = this.form.controls.destinationProgramId.value;

    this.originProgramOptions = this.programOptions.filter(option => option.value !== destinationId);
    this.destinationProgramOptions = this.programOptions.filter(option => option.value !== originId);

    if (originId && destinationId && originId === destinationId) {
      this.form.controls.destinationProgramId.setErrors({ sameProgram: true });
    } else if (this.form.controls.destinationProgramId.hasError('sameProgram')) {
      this.form.controls.destinationProgramId.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }
  }

  private updateSelectedFactor(): void {
    const originId = this.form.controls.originProgramId.value;
    const destinationId = this.form.controls.destinationProgramId.value;

    this.selectedFactor = originId && destinationId
      ? this.programFactors.find(factor =>
          factor.originProgramId === originId &&
          factor.destinationProgramId === destinationId
        ) ?? null
      : null;
  }

  private updateMilesReceived(): void {
    const milesSent = Number(this.form.controls.milesSent.value) || 0;
    const bonusPercentage = Number(this.form.controls.bonusPercentage.value) || 0;

    const milesAfterBonus = milesSent * (1 + bonusPercentage / 100);

    if (!this.selectedFactor) {
      this.form.controls.milesReceived.setValue(
        Math.round(milesAfterBonus),
        { emitEvent: false }
      );
      return;
    }

    const milesReceived =
      milesAfterBonus * (this.selectedFactor.toAmount / this.selectedFactor.fromAmount);

    this.form.controls.milesReceived.setValue(
      Math.round(milesReceived),
      { emitEvent: false }
    );
  }

  save(): void {
    if (this.form.invalid || !this.selectedFactor) {
      this.form.markAllAsTouched();
      return;
    }

    this.dialogRef.close(this.form.getRawValue());
  }

  cancel(): void {
    this.dialogRef.close();
  }

}
