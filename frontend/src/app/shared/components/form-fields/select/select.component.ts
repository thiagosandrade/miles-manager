import { CommonModule } from '@angular/common';
import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';

@Component({
  standalone: true,
  selector: 'app-select',
  imports: [CommonModule, MatFormFieldModule, MatSelectModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SelectComponent),
    multi: true
  }],
  styles: [`
    :host { display: block; }
    .full-width { width: 100%; }
  `],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>
      <mat-select
        [value]="value"
        [disabled]="disabled"
        (selectionChange)="onSelectionChange($event.value)">
        <mat-option *ngFor="let option of options" [value]="option.value">
          {{ option.viewValue }}
        </mat-option>
      </mat-select>
      <mat-error *ngIf="errorMessage && touched">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `
})
export class SelectComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() options: Array<{ value: unknown; viewValue: string }> = [];
  @Input() errorMessage = 'Invalid selection';
  @Output() selectionChange = new EventEmitter<unknown>();

  value: unknown = null;
  disabled = false;
  touched = false;

  private onChange: (value: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: unknown): void { this.value = value; }
  registerOnChange(fn: (value: unknown) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

  onSelectionChange(value: unknown): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
    this.touched = true;
    this.selectionChange.emit(value);
  }
}
