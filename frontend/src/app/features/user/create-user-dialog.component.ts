import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { TranslatePipe } from '@ngx-translate/core';

import { UserDetails } from './models/user.models';

export type UserDialogOperation = 'create' | 'update';

export interface UserDialogData {
  operation: UserDialogOperation;
  user?: UserDetails;
}

@Component({
  standalone: true,
  selector: 'app-user-dialog',
  templateUrl: './create-user-dialog.component.html',
  styleUrls: ['./create-user-dialog.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    TranslatePipe
  ]
})
export class CreateUserDialogComponent {

  private readonly fb = inject(FormBuilder);

  private readonly dialogRef =
    inject(MatDialogRef<CreateUserDialogComponent>);

  readonly data = inject<UserDialogData>(MAT_DIALOG_DATA);

  readonly form = this.fb.nonNullable.group({

    fullName: [
      this.data.user?.fullName ?? '',
      Validators.required
    ],

    email: [
      this.data.user?.email ?? '',
      [
        Validators.required,
        Validators.email
      ]
    ],

    password: [
      '',
      this.data.operation === 'create'
        ? Validators.required
        : []
    ],

    isActive: [
      this.data.user?.isActive ?? true
    ]

  });

  get isCreate(): boolean {
    return this.data.operation === 'create';
  }

  get user(): UserDetails | undefined {
    return this.data.user;
  }

  formatDate(value?: string | null): string {

    if (!value) {
      return '—';
    }

    return new Date(value).toLocaleString(undefined, {
      dateStyle: 'medium',
      timeStyle: 'short'
    });

  }

  save(): void {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();

    this.dialogRef.close({
      id: this.isCreate
      ? undefined
      : this.data.user?.id,
      fullName: value.fullName,
      email: value.email,
      password: value.password,
      isActive: value.isActive
    });

  }

  cancel(): void {

    this.dialogRef.close(null);

  }

}