import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  standalone: true,
  selector: 'app-money-input',
  imports: [CommonModule, MatFormFieldModule, MatInputModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => MoneyInputComponent),
    multi: true
  }],
  styles: [`
    :host { display: block; }
    .full-width { width: 100%; }
  `],
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
      <mat-error *ngIf="errorMessage && touched">{{ errorMessage }}</mat-error>
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
    }).formatToParts(0).find(part => part.type === 'currency')?.value ?? this.currency;
  }

  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | null): void { this.value = value; }
  registerOnChange(fn: (value: number | null) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

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
