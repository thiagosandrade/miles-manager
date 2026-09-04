import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TextInputComponent } from '../../shared/components/form-fields';
import { AuthApiService } from './auth-api.service';

@Component({
  standalone: true,
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
    TextInputComponent
  ]
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  form: FormGroup;
  loading = false;

  constructor() {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  submit(): void {
    if (this.form.invalid || this.loading) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.authApi.forgotPassword(this.form.getRawValue().email).subscribe({
      next: () => {
        this.snackBar.open(
          'If your user exists and is active, you should receive an email.',
          'Close',
          { duration: 5000 }
        );
      },
      error: () => {
        this.snackBar.open(
          'If your user exists and is active, you should receive an email.',
          'Close',
          { duration: 5000 }
        );
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth']);
  }
}
