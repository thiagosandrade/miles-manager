import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardApiService } from './dashboard-api.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DashboardDto } from './models/dashboard.model';
import { MatListModule } from "@angular/material/list";
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AppLoadingSpinnerComponent } from "../../shared/components/app-loading-spinner/app-loading-spinner.component";

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatTableModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatIconModule,
    MatButtonModule, AppLoadingSpinnerComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  private dashboardApi = inject(DashboardApiService);
  private cd = inject(ChangeDetectorRef);
  
  loading = true;
  error = '';
  stats: DashboardDto | null = null;

  displayedColumns: string[] = ['name', 'value', 'description'];
  dataSource = [];

  ngOnInit() {
    this.loadDashboard();
  }

  async loadDashboard() {
    this.loading = true;
    this.error = '';
    try {
      const response = await this.dashboardApi.getDashboard();
      this.stats = response;
      this.loading = false;
      this.cd.detectChanges();
    } catch (err: any) {
      this.error = err.message || 'Error loading dashboard statistics';
      this.loading = false;
    }
  }
}

