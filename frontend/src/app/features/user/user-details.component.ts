import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserApiService } from './user-api.service';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserDetails } from './models/user.models';
import { AppLoadingSpinnerComponent } from "../../shared/components/app-loading-spinner/app-loading-spinner.component";
import { MatIconModule } from "@angular/material/icon";

@Component({
  standalone: true,
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss'],
  imports: [CommonModule, MatCardModule, MatProgressSpinnerModule, AppLoadingSpinnerComponent, MatIconModule]
})
export class UserDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private userApi = inject(UserApiService);
  private cd = inject(ChangeDetectorRef);

  userId: string | null = null;
  user: UserDetails | null = null;
  loading = true;
  error = '';

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id');
    if (this.userId) {
      this.loadUserDetails(this.userId);
    } else {
      this.error = 'No user ID provided';
      this.loading = false;
    }
  }

  async loadUserDetails(id: string): Promise<void> {
    this.loading = true;
    this.error = '';
    try {
      this.user = await this.userApi.getUser(id);
    } catch (err: any) {
      this.error = err.message || 'Failed to load user details';
      this.user = null;
    } finally {
      this.loading = false;
      this.cd.detectChanges();
    }
  }
}
