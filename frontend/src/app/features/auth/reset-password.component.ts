import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
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
    TextInputComponent,
    TranslatePipe
  ]
})
export class ResetPasswordComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private translate = inject(TranslateService);

  form: FormGroup;
  token = '';
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly validToken = signal(false);
  
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
      this.validToken.set(false);
      this.loading.set(true);

      if (!token) {
        this.loading.set(false);
        return;
      }

      this.authApi.validatePasswordResetToken(token).subscribe({
        next: response => {
          if (response.valid && response.email) {
            this.validToken.set(true);
            this.form.patchValue({ email: response.email });
          }

          this.loading.set(false);
        },
        error: () => {
          this.validToken.set(false);
          this.loading.set(false);
        }
      });
    });
  }

  submit(): void {
    if (this.form.invalid || this.saving() || !this.validToken()) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.authApi.resetPassword(this.token, this.form.getRawValue().newPassword).subscribe({
      next: () => {
        this.snackBar.open(
          this.translate.instant('resetPassword.success'),
          this.translate.instant('common.close'),
          { duration: 4000 }
        );
        setTimeout(() => this.router.navigate(['/auth']), 1500);
      },
      error: err => {
        this.saving.set(false);
        this.snackBar.open(
          err?.status === 401
            ? this.translate.instant('resetPassword.invalidLink')
            : this.translate.instant('resetPassword.updateError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
      },
      complete: () => {
        this.saving.set(false);
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth']);
  }
}
