import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { AppTableComponent } from '../../shared/components/app-table/app-table.component';
import { SnackbarService } from '../../core/services/snackbar.service';
import { AlertSource } from './models/alert-source.model';
import { AlertsApiService } from './alerts-api.service';
import { UpdateAlertSourceDialogComponent } from './update-alert-source-dialog.component';

@Component({
  selector: 'app-alert-sources-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatInputModule,
    MatTableModule,
    TranslatePipe,
    AppTableComponent,
  ],
  templateUrl: './alert-sources-page.component.html',
  styleUrls: ['./alert-sources-page.component.scss'],
})
export class AlertSourcesPageComponent implements OnInit {
  private readonly api = inject(AlertsApiService);
  private readonly fb = inject(FormBuilder);
  private readonly dialog = inject(MatDialog);
  private readonly snackbar = inject(SnackbarService);

  readonly sources = signal<AlertSource[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);

  readonly displayedColumns = ['title', 'country', 'externalUrl', 'createdAtUtc', 'actions'];

  readonly form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    externalUrl: ['', [Validators.required, Validators.pattern(/^https?:\/\/[^\s]+$/i)]],
    country: ['', Validators.required],
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);

    this.api.getAlertSources().subscribe({
      next: sources => {
        this.sources.set(sources);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackbar.error('Could not load alert sources.', 3000);
      },
    });
  }

  create(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.api.createAlertSource(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset();
        this.saving.set(false);
        this.snackbar.success('Alert source added successfully.', 3000);
        this.load();
      },
      error: error => {
        this.saving.set(false);
        this.snackbar.error(
          error?.error?.title ?? 'Could not add alert source.',
          3000,
        );
      },
    });
  }

  edit(source: AlertSource): void {
    const dialogRef = this.dialog.open(UpdateAlertSourceDialogComponent, {
      width: '560px',
      maxWidth: '95vw',
      data: source,
    });

    dialogRef.afterClosed().subscribe(updated => {
      if (updated) {
        this.load();
      }
    });
  }

  remove(source: AlertSource): void {
    this.saving.set(true);
    this.api.deleteAlertSource(source.id).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackbar.success('Alert source removed.', 3000);
        this.load();
      },
      error: () => {
        this.saving.set(false);
        this.snackbar.error('Could not remove alert source.', 3000);
      },
    });
  }

  trackById(_: number, source: AlertSource): string {
    return source.id;
  }
}
