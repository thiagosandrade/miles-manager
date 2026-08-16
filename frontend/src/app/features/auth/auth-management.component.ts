import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthApiService } from './auth-api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  standalone: true,
  selector: 'app-auth-management',
  templateUrl: './auth-management.component.html',
  styleUrls: ['./auth-management.component.scss'],
  imports: [
    CommonModule, ReactiveFormsModule, MatInputModule, MatButtonModule, 
    MatCardModule, MatSnackBarModule, MatIconModule,
    TranslatePipe
],
})
export class AuthManagementComponent {
  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private tokenStorage = inject(TokenStorageService);
  form: FormGroup;
  loading = false;

  constructor() {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  submit() {
    if (this.form.valid) {
      this.loading = true;
      this.authApi.login(this.form.value).subscribe({
        next: (res) => {

          this.tokenStorage.saveToken(res.token);
          this.snackBar.open('Login successful', 'Close', { duration: 3000 });
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.loading = false;
          this.snackBar.open('Login failed: ' + (err?.error?.message || err.message), 'Close', { duration: 5000 });
        },
        complete: () => {
          this.loading = false;
        },
      });
    }
  }
}
