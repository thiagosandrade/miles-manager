import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { AlertsApiService } from './alerts-api.service';
import { AlertSource, AlertSourceUpdateRequest } from './models/alert-source.model';
import { SnackbarService } from '../../core/services/snackbar.service';

@Component({
  selector: 'app-update-alert-source-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './update-alert-source-dialog.component.html',
  styleUrls: ['./update-alert-source-dialog.component.scss'],
})
export class UpdateAlertSourceDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(AlertsApiService);
  private readonly dialogRef = inject(MatDialogRef<UpdateAlertSourceDialogComponent>);
  private readonly snackbar = inject(SnackbarService);
  readonly source = inject<AlertSource>(MAT_DIALOG_DATA);

  saving = false;

  readonly form = this.fb.nonNullable.group({
    title: [this.source.title, Validators.required],
    externalUrl: [
      this.source.externalUrl,
      [Validators.required, Validators.pattern(/^https?:\/\/[^\s]+$/i)],
    ],
    country: [this.source.country, Validators.required],
  });

  save(): void {
    if (this.form.invalid || this.saving) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const request: AlertSourceUpdateRequest = this.form.getRawValue();

    this.api.updateAlertSource(this.source.id, request).subscribe({
      next: () => {
        this.saving = false;
        this.snackbar.success('Alert source updated successfully.', 3000);
        this.dialogRef.close(true);
      },
      error: error => {
        this.saving = false;
        this.snackbar.error(
          error?.error?.title ?? 'Could not update alert source.',
          3000,
        );
      },
    });
  }

  close(): void {
    this.dialogRef.close(false);
  }
}
