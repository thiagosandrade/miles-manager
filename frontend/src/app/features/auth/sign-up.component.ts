import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { TextInputComponent } from '../../shared/components/form-fields';
import { AuthApiService } from './auth-api.service';

@Component({
  standalone: true,
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss'],
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
export class SignUpComponent {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private translate = inject(TranslateService);

  form: FormGroup;
  readonly loading = signal(false);

  constructor() {
    this.form = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  submit(): void {
    if (this.form.invalid || this.loading()) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.authApi.register(this.form.getRawValue()).subscribe({
      next: () => {
        this.snackBar.open(
          this.translate.instant('signup.pendingApproval'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );

        setTimeout(() => this.router.navigate(['/auth']), 3000);
      },
      error: (err) => {
        this.loading.set(false);
        const message = err?.status === 409
          ? this.translate.instant('signup.emailExists')
          : (err?.error?.message || this.translate.instant('signup.registrationFailed'));
        this.snackBar.open(message, this.translate.instant('common.close'), { duration: 5000 });
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/auth']);
  }
}
