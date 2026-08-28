import { CommonModule } from '@angular/common';
import { Component, EventEmitter, forwardRef, inject, Input, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TranslatePipe } from '@ngx-translate/core';

export interface SelectOption<T = string> {
  value: T;
  label: string;
}

@Component({
  standalone: true,
  selector: 'app-select',
  imports: [CommonModule, MatFormFieldModule, MatSelectModule, TranslatePipe],
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
          {{ option.label | translate }}
        </mat-option>
      </mat-select>
      <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `
})
export class SelectComponent<T = string> implements ControlValueAccessor {
  @Input() label = '';
  @Input() options: SelectOption<T>[] = [];
  @Input() errors: Record<string, string> = {};
  @Output() selectionChange = new EventEmitter<T>();

  value: T | null = null;
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

  private onChange: (value: T) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: T | null): void { this.value = value; }
  registerOnChange(fn: (value: T) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

  onSelectionChange(value: T): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
    this.touched = true;
    this.selectionChange.emit(value);
  }
}
