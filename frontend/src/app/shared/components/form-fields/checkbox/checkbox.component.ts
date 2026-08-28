import { CommonModule } from '@angular/common';
import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  standalone: true,
  selector: 'app-checkbox',
  imports: [CommonModule, MatCheckboxModule],
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

  writeValue(value: boolean | null): void { this.value = value ?? false; }
  registerOnChange(fn: (value: boolean) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

  onCheckboxChange(value: boolean): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
  }
}
