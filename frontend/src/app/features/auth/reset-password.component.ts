import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TextInputComponent } from '../../shared/components/form-fields';
import { AuthApiService } from './auth-api.service';

function passwordsMatch(control: AbstractControl): ValidationErrors | null {
  const password = control.get('newPassword')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password && confirmPassword && password !== confirmPassword
    ? { passwordsMismatch: true }
    : null;
}

@Component({
  standalone: true,
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
    TextInputComponent
  ]
})
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  form: FormGroup;
  token = '';
  loading = true;
  saving = false;
  validToken = false;

  constructor() {
    this.form = this.fb.group({
      email: [{ value: '', disabled: true }],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordsMatch });
  }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      const token = params.get('token') ?? '';
      this.token = token;
      this.validToken = false;
      this.loading = true;

      if (!token) {
        this.loading = false;
        return;
      }

      this.authApi.validatePasswordResetToken(token).subscribe({
        next: response => {
          if (response.valid && response.email) {
            this.validToken = true;
            this.form.patchValue({ email: response.email });
          }

          this.loading = false;
        },
        error: () => {
          this.validToken = false;
          this.loading = false;
        }
      });
    });
  }

  submit(): void {
    if (this.form.invalid || this.saving || !this.validToken) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.authApi.resetPassword(this.token, this.form.getRawValue().newPassword).subscribe({
      next: () => {
        this.snackBar.open('Your password has been updated successfully.', 'Close', { duration: 4000 });
        setTimeout(() => this.router.navigate(['/auth']), 1500);
      },
      error: err => {
        this.saving = false;
        this.snackBar.open(
          err?.status === 401
            ? 'This password reset link is invalid or has expired.'
            : 'Could not update your password. Please try again.',
          'Close',
          { duration: 5000 }
        );
      },
      complete: () => {
        this.saving = false;
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth']);
  }
}
