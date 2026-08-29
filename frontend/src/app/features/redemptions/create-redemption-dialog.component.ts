import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { Program } from '../programs/models/program.model';
import { ProgramsApiService } from '../programs/programs-api.service';
import { FlightRedemptionService } from './flight-redemption.service';
import { FlightRedemption } from './models/flight-redemption.model';
import { AirportsDTO, LookupService } from '../../core/services/dataapp-api.service';
import { TranslatePipe } from '@ngx-translate/core';
import {
  CheckboxComponent,
  DateInputComponent,
  MoneyInputComponent,
  NumberInputComponent,
  SelectComponent,
  type SelectOption
} from '../../shared/components/form-fields/form-fields';

export interface RedemptionDialogData {
  operation: 'create' | 'update';
  redemptionId?: string;
  redemption?: FlightRedemption;
}

@Component({
  selector: 'app-create-redemption-dialog',
  standalone: true,
  templateUrl: './create-redemption-dialog.component.html',
  styleUrls: ['./create-redemption-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    SelectComponent,
    DateInputComponent,
    NumberInputComponent,
    MoneyInputComponent,
    CheckboxComponent,
    TranslatePipe
  ]
})
export class CreateRedemptionDialogComponent {

  private fb = inject(FormBuilder);
  private programsApi = inject(ProgramsApiService);
  private redemptionService = inject(FlightRedemptionService);
  private dataAppService = inject(LookupService);

  readonly data = inject<RedemptionDialogData>(MAT_DIALOG_DATA);
  airports: AirportsDTO[] = [];
  airportOptions: SelectOption<string>[] = [];

  private dialogRef = inject(MatDialogRef<CreateRedemptionDialogComponent>);

  programs: Program[] = [];
  programOptions: SelectOption<string>[] = [];
  readonly cabinClassOptions: SelectOption<number>[] = [
    { value: 0, label: 'redemptions.cabin.economy' },
    { value: 1, label: 'redemptions.cabin.premiumEconomy' },
    { value: 2, label: 'redemptions.cabin.business' },
    { value: 3, label: 'redemptions.cabin.first' }
  ];

  readonly form = this.fb.group({
    id: [''],
    programId: ['', Validators.required],
    originAirportCode: ['', Validators.required],
    destinationAirportCode: ['', Validators.required],
    flightDate: [new Date(), Validators.required],
    cabinClass: [0, Validators.required],
    passengers: [1, [Validators.required, Validators.min(1)]],
    isRoundTrip: false,
    milesUsed: [0, [Validators.required, Validators.min(1)]],
    taxes: [0, [Validators.required, Validators.min(0)]],
    cashPrice: [0, Validators.min(0)]
  });

  ngOnInit(): void {

    this.dataAppService.getAirports().then(result => {
      this.airports = result;
      this.airportOptions = this.airports.map(airport => ({
        value: airport.iataCode,
        label: `${airport.name} (${airport.iataCode})`
      }));
    });

    this.programsApi.getPrograms().then(result => {
      this.programs = result.programs.filter(p => p.isActive);
      this.programOptions = this.programs.map(program => ({
        value: program.id,
        label: program.name
      }));

      if (this.data.operation === 'update' && this.data.redemptionId) {
        if (this.data.redemption) {
          this.patchRedemption(this.data.redemption);
          return;
        }

        this.redemptionService.getFlightRedemption(this.data.redemptionId).subscribe({
          next: redemption => this.patchRedemption(redemption)
        });
      }
    });

  }

  private patchRedemption(redemption: FlightRedemption): void {
    this.form.patchValue({
      id: redemption.id,
      programId: this.programs.find(p => p.name === redemption.program)?.id,
      originAirportCode: redemption.originAirportCode,
      destinationAirportCode: redemption.destinationAirportCode,
      flightDate: new Date(redemption.flightDate),
      cabinClass: redemption.cabinClass,
      passengers: redemption.passengers,
      isRoundTrip: redemption.isRoundTrip,
      milesUsed: redemption.milesUsed,
      taxes: redemption.taxes,
      cashPrice: redemption.cashPrice
    });
  }

  save(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;

    }

    const value = this.form.getRawValue();

    this.dialogRef.close({
      ...value,
      cashPrice: value.cashPrice ?? 0
    });

  }

  cancel(): void {

    this.dialogRef.close();

  }

}
