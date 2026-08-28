import { CommonModule } from '@angular/common';
import { Component, forwardRef, inject, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  standalone: true,
  selector: 'app-text-input',
  imports: [CommonModule, MatFormFieldModule, MatInputModule, MatIconModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => TextInputComponent),
    multi: true
  }],
  styles: [`
    :host { display: block; }
    .full-width { width: 100%; }
  `],
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
      <mat-error *ngIf="errorMessage">{{ errorMessage }}</mat-error>
    </mat-form-field>
  `
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() icon = '';
  @Input() errors: Record<string, string> = {};

  value = '';
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

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void { this.value = value ?? ''; }
  registerOnChange(fn: (value: string) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(isDisabled: boolean): void { this.disabled = isDisabled; }

  onInput(event: Event): void {
    this.value = (event.target as HTMLInputElement).value;
    this.onChange(this.value);
  }

  onBlur(): void {
    this.touched = true;
    this.onTouched();
  }
}
