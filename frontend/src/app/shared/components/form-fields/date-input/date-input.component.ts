import { CommonModule } from '@angular/common';
import { Component, forwardRef, inject, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  standalone: true,
  selector: 'app-date-input',
  imports: [CommonModule, MatFormFieldModule, MatInputModule, MatDatepickerModule, MatNativeDateModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DateInputComponent),
    multi: true
  }],
  styles: [`
    :host { display: block; }
    .full-width { width: 100%; }
  `],
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
      <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
      <mat-datepicker #picker></mat-datepicker>
      <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `
})
export class DateInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() errors: Record<string, string> = {};
  @Input() min: Date | null = null;
  @Input() max: Date | null = null;

  value: Date | null = null;
  disabled = false;
  touched = false;

  private readonly ngControl = inject(NgControl, { self: true, optional: true });

  get errorMessage(): string | null {
    if (!this.touched && !this.ngControl?.control?.touched) {
      return null;
    }

    const controlErrors = this.ngControl?.control?.errors ?? {};
    const errorKey = Object.keys(controlErrors).find(key => this.errors[key]);

    return errorKey ? this.errors[errorKey] : null;
  }

  private onChange: (value: Date | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: Date | string | null): void { this.value = value ? new Date(value) : null; }
  registerOnChange(fn: (value: Date | null) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

  onDateChange(value: Date | null): void {
    this.value = value;
    this.onChange(value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}
