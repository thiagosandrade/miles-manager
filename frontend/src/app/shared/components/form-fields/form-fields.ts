import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgControl, NG_VALUE_ACCESSOR, ControlValueAccessor, ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  standalone: true,
  selector: 'app-text-input',
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>
      <input matInput [formControl]="control" [type]="type" />
      <mat-error *ngIf="control.invalid && control.touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class TextInputComponent {
  @Input() label = '';
  @Input() type = 'text';
  @Input() control!: FormControl;
  @Input() errorMessage = 'Invalid input';
}

@Component({
  standalone: true,
  selector: 'app-select',
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>
      <mat-select [formControl]="control">
        <mat-option *ngFor="let option of options" [value]="option.value">
          {{ option.viewValue }}
        </mat-option>
      </mat-select>
      <mat-error *ngIf="control.invalid && control.touched">
        {{ errorMessage }}
      </mat-error>
    </mat-form-field>
  `
})
export class SelectComponent {
  @Input() label = '';
  @Input() control!: FormControl;
  @Input() options: Array<{ value: any; viewValue: string }> = [];
  @Input() errorMessage = 'Invalid selection';
}

@Component({
  standalone: true,
  selector: 'app-checkbox',
  imports: [CommonModule, ReactiveFormsModule, MatCheckboxModule],
  template: `
    <mat-checkbox [formControl]="control">{{ label }}</mat-checkbox>
  `
})
export class CheckboxComponent {
  @Input() label = '';
  @Input() control!: FormControl;
}
