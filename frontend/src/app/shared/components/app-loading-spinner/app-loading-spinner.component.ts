import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  standalone: true,
  selector: 'app-loading-spinner',
  imports: [CommonModule, MatProgressSpinnerModule],
  templateUrl: './app-loading-spinner.component.html',
  styleUrls: ['./app-loading-spinner.component.css']
})
export class AppLoadingSpinnerComponent {}
