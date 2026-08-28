import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

import { TranslatePipe } from '@ngx-translate/core';

import {
  CompareFlightRequest,
  FlightComparisonDto
} from './models/flight-comparison.model';

import { CalculatorApiService } from './calculator-api.service';
import { ProgramsApiService } from '../programs/programs-api.service';
import { Program } from '../programs/models/program.model';

import { firstValueFrom } from 'rxjs';
import {
  MoneyInputComponent,
  NumberInputComponent,
  SelectComponent,
  type SelectOption
} from '../../shared/components/form-fields';

@Component({
  selector: 'app-calculator',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MoneyInputComponent,
    NumberInputComponent,
    SelectComponent,
    TranslatePipe
  ],
  templateUrl: './calculator.component.html',
  styleUrls: ['./calculator.component.scss']
})
export class CalculatorComponent implements OnInit {

  private readonly fb = inject(FormBuilder);
  private readonly calculatorApi = inject(CalculatorApiService);
  private readonly programApi = inject(ProgramsApiService);

  readonly Math = Math;

  // -----------------------------
  // State
  // -----------------------------

  readonly programs = signal<Program[]>([]);
  programOptions: SelectOption<string>[] = [];

  readonly loading = signal(false);
  readonly loadingPrograms = signal(false);
  readonly error = signal('');

  readonly result = signal<FlightComparisonDto | null>(null);

  readonly selectedCurrency = signal<string | null>(null);

  // -----------------------------
  // Form
  // -----------------------------

  readonly form = this.fb.nonNullable.group({

    programId: [
      '',
      Validators.required
    ],

    cashPrice: [
      0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ],

    milesRequired: [
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
    ]

  });

  // -----------------------------
  // Lifecycle
  // -----------------------------

  ngOnInit(): void {
    this.loadPrograms();
  }

  // -----------------------------
  // Programs
  // -----------------------------

  loadPrograms(): void {

    this.loadingPrograms.set(true);

    this.programApi.getPrograms()
      .then(response => {

        const activePrograms = response.programs.filter(
          program => program.isActive
        );

        this.programs.set(activePrograms);
        this.programOptions = activePrograms.map(program => ({
          value: program.id,
          label: `${program.name} (${program.currencyCode})`
        }));

        this.loadingPrograms.set(false);

      })
      .catch(() => {

        this.loadingPrograms.set(false);

        this.error.set(
          'Could not load loyalty programs.'
        );

      });

  }

  onProgramChange(programId: string): void {

    const program = this.programs().find(
      x => x.id === programId
    );

    this.selectedCurrency.set(
      program?.currencyCode ?? null
    );

  }

  getCurrencySymbol(currencyCode: string): string {

    switch (currencyCode) {

      case 'EUR':
        return '€';

      case 'USD':
        return '$';

      case 'BRL':
        return 'R$';

      default:
        return currencyCode;

    }

  }

  // -----------------------------
  // Comparison
  // -----------------------------

  async compare(): Promise<void> {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;

    }

    this.loading.set(true);
    this.error.set('');
    this.result.set(null);

    const request: CompareFlightRequest = {

      programId:
        this.form.controls.programId.value,

      cashPrice:
        this.form.controls.cashPrice.value,

      milesRequired:
        this.form.controls.milesRequired.value,

      taxes:
        this.form.controls.taxes.value

    };

    try {

      const result = await firstValueFrom(
        this.calculatorApi.compare(request)
      );

      this.result.set(result);

    } catch (err: any) {

      this.error.set(
        err?.error?.message ??
        'Could not compare the flight options.'
      );

    } finally {

      this.loading.set(false);

    }

  }

  // -----------------------------
  // Reset
  // -----------------------------

  reset(): void {

    this.form.reset({
      programId: '',
      cashPrice: 0,
      milesRequired: 0,
      taxes: 0
    });

    this.selectedCurrency.set(null);
    this.result.set(null);
    this.error.set('');

  }

  // -----------------------------
  // Currency
  // -----------------------------

  formatCurrency(
    value: number,
    currencyCode: string
  ): string {

    return new Intl.NumberFormat(
      undefined,
      {
        style: 'currency',
        currency: currencyCode
      }
    ).format(value);

  }

}
