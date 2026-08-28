import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import {
  ControlValueAccessor,
  FormsModule,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule
} from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

const VALUE_ACCESSOR = {
  provide: NG_VALUE_ACCESSOR,
  useExisting: forwardRef(() => TextInputComponent),
  multi: true
};

const baseFieldStyles = `
  :host {
    display: block;
  }

  .full-width {
    width: 100%;
  }
`;

@Component({
  standalone: true,
  selector: 'app-text-input',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  providers: [VALUE_ACCESSOR],
  styles: [baseFieldStyles],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>

      <mat-icon *ngIf="icon" matPrefix>{{ icon }}</mat-icon>

      <input
        matInput
        [type]="type"
        [placeholder]="placeholder"
        [value]="value"
        [disabled]="disabled"
        (input)="onInput($event)"
        (blur)="onBlur()">

      <mat-error *ngIf="errorMessage && touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() icon = '';
  @Input() errorMessage = 'Invalid input';

  value = '';
  disabled = false;
  touched = false;

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value = value ?? '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    this.value = (event.target as HTMLInputElement).value;
    this.onChange(this.value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}

@Component({
  standalone: true,
  selector: 'app-number-input',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => NumberInputComponent),
    multi: true
  }],
  styles: [baseFieldStyles],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>

      <mat-icon *ngIf="icon" matPrefix>{{ icon }}</mat-icon>

      <input
        matInput
        type="number"
        [placeholder]="placeholder"
        [value]="value ?? ''"
        [disabled]="disabled"
        [min]="min"
        [max]="max"
        [step]="step"
        (input)="onInput($event)"
        (blur)="onBlur()">

      <mat-error *ngIf="errorMessage && touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class NumberInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() icon = '';
  @Input() errorMessage = 'Invalid number';
  @Input() min: number | null = null;
  @Input() max: number | null = null;
  @Input() step: number | 'any' = 'any';

  value: number | null = null;
  disabled = false;
  touched = false;

  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    const rawValue = (event.target as HTMLInputElement).value;
    this.value = rawValue === '' ? null : Number(rawValue);
    this.onChange(this.value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}

@Component({
  standalone: true,
  selector: 'app-money-input',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => MoneyInputComponent),
    multi: true
  }],
  styles: [baseFieldStyles],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>

      <span matTextPrefix>{{ currencySymbol }}&nbsp;</span>

      <input
        matInput
        type="number"
        inputmode="decimal"
        [placeholder]="placeholder"
        [value]="value ?? ''"
        [disabled]="disabled"
        [min]="min"
        [step]="step"
        (input)="onInput($event)"
        (blur)="onBlur()">

      <mat-error *ngIf="errorMessage && touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class MoneyInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() currency = 'EUR';
  @Input() locale = 'en-US';
  @Input() errorMessage = 'Invalid amount';
  @Input() min: number | null = 0;
  @Input() step = 0.01;

  value: number | null = null;
  disabled = false;
  touched = false;

  get currencySymbol(): string {
    return new Intl.NumberFormat(this.locale, {
      style: 'currency',
      currency: this.currency,
      currencyDisplay: 'narrowSymbol'
    })
      .formatToParts(0)
      .find(part => part.type === 'currency')?.value ?? this.currency;
  }

  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | null): void {
    this.value = value;
  }

  registerOnChange(fn: (value: number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    const rawValue = (event.target as HTMLInputElement).value;
    this.value = rawValue === '' ? null : Number(rawValue);
    this.onChange(this.value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}

@Component({
  standalone: true,
  selector: 'app-date-input',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DateInputComponent),
    multi: true
  }],
  styles: [baseFieldStyles],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>

      <input
        matInput
        [matDatepicker]="picker"
        [placeholder]="placeholder"
        [value]="value"
        [disabled]="disabled"
        [min]="min"
        [max]="max"
        (dateChange)="onDateChange($event.value)"
        (blur)="onBlur()">

      <mat-datepicker-toggle
        matIconSuffix
        [for]="picker">
      </mat-datepicker-toggle>

      <mat-datepicker #picker></mat-datepicker>

      <mat-error *ngIf="errorMessage && touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class DateInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() errorMessage = 'Invalid date';
  @Input() min: Date | null = null;
  @Input() max: Date | null = null;

  value: Date | null = null;
  disabled = false;
  touched = false;

  private onChange: (value: Date | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: Date | string | null): void {
    this.value = value ? new Date(value) : null;
  }

  registerOnChange(fn: (value: Date | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onDateChange(value: Date | null): void {
    this.value = value;
    this.onChange(value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}

@Component({
  standalone: true,
  selector: 'app-select',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule
  ],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SelectComponent),
    multi: true
  }],
  styles: [baseFieldStyles],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>

      <mat-select
        [value]="value"
        [disabled]="disabled"
        (selectionChange)="onSelectionChange($event.value)">

        <mat-option
          *ngFor="let option of options"
          [value]="option.value">
          {{ option.viewValue }}
        </mat-option>

      </mat-select>

      <mat-error *ngIf="errorMessage && touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class SelectComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() options: Array<{ value: unknown; viewValue: string }> = [];
  @Input() errorMessage = 'Invalid selection';

  value: unknown = null;
  disabled = false;
  touched = false;

  private onChange: (value: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: unknown): void {
    this.value = value;
  }

  registerOnChange(fn: (value: unknown) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSelectionChange(value: unknown): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
    this.touched = true;
  }
}

@Component({
  standalone: true,
  selector: 'app-checkbox',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatCheckboxModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => CheckboxComponent),
    multi: true
  }],
  template: `
    <mat-checkbox
      [checked]="value"
      [disabled]="disabled"
      (change)="onCheckboxChange($event.checked)">
      {{ label }}
    </mat-checkbox>
  `
})
export class CheckboxComponent implements ControlValueAccessor {
  @Input() label = '';

  value = false;
  disabled = false;

  private onChange: (value: boolean) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: boolean | null): void {
    this.value = value ?? false;
  }

  registerOnChange(fn: (value: boolean) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onCheckboxChange(value: boolean): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
  }
}
