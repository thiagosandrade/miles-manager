import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {

  private snackBar = inject(MatSnackBar);

  success(message: string, duration = 3000): void {
    this.open(message, 'success-snackbar', duration);
  }

  error(message: string, duration = 5000): void {
    this.open(message, 'error-snackbar', duration);
  }

  warning(message: string, duration = 4000): void {
    this.open(message, 'warning-snackbar', duration);
  }

  info(message: string, duration = 3000): void {
    this.open(message, 'info-snackbar', duration);
  }

  private open(
    message: string,
    panelClass: string,
    duration: number
  ): void {

    this.snackBar.open(message, undefined, {
      duration,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: [panelClass]
    });

  }

}