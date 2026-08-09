import { Component, Output, EventEmitter, inject, signal } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserApiService } from '../../features/user/user-api.service';
import { MatDivider } from "@angular/material/divider";
import { AuthApiService } from '../../features/auth/auth-api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';

@Component({
  standalone: true,
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css'],
  imports: [CommonModule, MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule, MatDivider]
})

export class ToolbarComponent {
  private userApi = inject(UserApiService);
  private authApi = inject(AuthApiService);
  private tokenStorage = inject(TokenStorageService);
  private router = inject(Router);

  userName = signal('');
  userEmail = signal('');

  isActive(path: string): boolean {
    return this.router.url === path;
  }

  constructor() {
    this.loadUserInfo();
  }

  private async loadUserInfo(): Promise<void> {
    const id = this.currentUserId();

    if (!id) {
        return;
    }

    try {
        const user = await this.userApi.getUser(id);

        this.userName.set(user.fullName);
        this.userEmail.set(user.email);
    }
    catch {
        this.userName.set('');
        this.userEmail.set('');
    }
  }

  private currentUserId(): string {
    const user = this.tokenStorage.getUser();
    if (user && user.id) {
      return user.id;
    }
    return '';
  }

  logout(): void {
    this.authApi.logout();
    this.router.navigate(['/auth']);
  }

  userDetails(): void {
    const userId = this.currentUserId();
    if (userId) {
      this.router.navigate([`/users/${userId}`]);
    }
  }

  switchTheme(): void {
    const currentTheme = document.body.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    document.body.setAttribute('data-theme', newTheme);
  }
}
