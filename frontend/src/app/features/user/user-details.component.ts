import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';

import { UserApiService } from './user-api.service';
import { UserDetails } from './models/user.models';

@Component({
  standalone: true,
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.scss'],
  imports: [
    CommonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatIconModule,
    TranslatePipe
  ]
})
export class UserDetailsComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly userApi = inject(UserApiService);

  readonly loading = signal(true);
  readonly user = signal<UserDetails | null>(null);
  readonly error = signal('');

  ngOnInit(): void {

    this.route.paramMap.subscribe(params => {

      const id = params.get('id');

      if (!id) {
        this.error.set('No user ID provided');
        this.loading.set(false);
        return;
      }

      this.loadUser(id);

    });

  }

  private loadUser(id: string): void {

    this.loading.set(true);
    this.error.set('');
    this.user.set(null);

    this.userApi.getUser(id).subscribe({

      next: user => {

        this.user.set(user);
        this.loading.set(false);

      },

      error: err => {

        this.error.set(err?.message ?? 'Failed to load user');
        this.loading.set(false);

      }

    });

  }

}