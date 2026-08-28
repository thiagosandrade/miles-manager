import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, Input } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  standalone: true,
  selector: 'app-money-input',
  imports: [CommonModule, MatFormFieldModule, MatInputModule],
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
      <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `
})
export class MoneyInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() currency = 'EUR';
  @Input() locale = 'en-US';
  @Input() errors: Record<string, string> = {};
  @Input() min: number | null = 0;
  @Input() step = 0.01;

  value: number | null = null;
  disabled = false;
  touched = false;

  private readonly ngControl = inject(NgControl, { self: true, optional: true });
  private readonly changeDetector = inject(ChangeDetectorRef);

  constructor() {
    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  get currencySymbol(): string {
    return new Intl.NumberFormat(this.locale, {
      style: 'currency',
      currency: this.currency,
      currencyDisplay: 'narrowSymbol'
    }).formatToParts(0).find(part => part.type === 'currency')?.value ?? this.currency;
  }

  get errorMessage(): string | null {
    if (!this.touched && !this.ngControl?.control?.touched) {
      return null;
    }

    const controlErrors = this.ngControl?.control?.errors ?? {};
    const errorKey = Object.keys(controlErrors).find(key => this.errors[key]);

    return errorKey ? this.errors[errorKey] : null;
  }

  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | null): void {
    this.value = value;
    this.changeDetector.markForCheck();
  }
  registerOnChange(fn: (value: number | null) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
    this.changeDetector.markForCheck();
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
