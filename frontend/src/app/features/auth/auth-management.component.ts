import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthApiService } from './auth-api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TextInputComponent } from '../../shared/components/form-fields';

@Component({
  standalone: true,
  selector: 'app-auth-management',
  templateUrl: './auth-management.component.html',
  styleUrls: ['./auth-management.component.scss'],
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
export class AuthManagementComponent {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private tokenStorage = inject(TokenStorageService);
  private translate = inject(TranslateService);
  form: FormGroup;
  readonly loading = signal(false);

  constructor() {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  submit() {
    if (this.form.valid) {
      this.loading.set(true);
      this.authApi.login(this.form.value).subscribe({
        next: (res) => {
          this.tokenStorage.saveToken(res.token);
          this.snackBar.open(this.translate.instant('auth.loginSuccess'), this.translate.instant('common.close'), { duration: 3000 });
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.loading.set(false);
          this.snackBar.open(
            this.translate.instant('auth.loginFailed', { message: err?.error?.message || err.message }),
            this.translate.instant('common.close'),
            { duration: 5000 }
          );
        },
        complete: () => {
          this.loading.set(false);
        }
      });
    }
  }

  goToSignUp(): void {
    this.router.navigate(['/auth/sign-up']);
  }

  goToForgotPassword(): void {
    this.router.navigate(['/auth/forgot-password']);
  }
}
