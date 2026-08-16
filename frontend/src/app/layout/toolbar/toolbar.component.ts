import { Component, Output, EventEmitter, inject, signal } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatDivider } from "@angular/material/divider";
import { AuthApiService } from '../../features/auth/auth-api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { LanguageService } from '../../core/services/language.service';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  standalone: true,
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.css'],
  imports: [
    CommonModule, 
    MatToolbarModule, 
    MatButtonModule, 
    MatIconModule, 
    MatMenuModule, 
    MatDivider,
    TranslatePipe
  ]
})

export class ToolbarComponent {
  private authApi = inject(AuthApiService);
  private tokenStorage = inject(TokenStorageService);
  private router = inject(Router);

  readonly user = this.tokenStorage.getUser();

  languageService = inject(LanguageService);

  languages = [
    { code: 'en', name: 'English', flag: '🇺🇸' },
    { code: 'pt-BR', name: 'Português (Brasil)', flag: '🇧🇷' },
    { code: 'pt-PT', name: 'Português', flag: '🇵🇹' },
    { code: 'es', name: 'Español', flag: '🇪🇸' },
    { code: 'fr', name: 'Français', flag: '🇫🇷' },
    { code: 'de', name: 'Deutsch', flag: '🇩🇪' }
  ];

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

    this.userName.set(this.user.fullName);
    this.userEmail.set(this.user.email);
  }

  private currentUserId(): string {
    if (this.user && this.user.id) {
      return this.user.id;
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

  get currentLanguage(): string {
    return this.languageService.currentLanguage;
  }

  get currentLanguageName(): string {
    return this.languages.find(x => x.code === this.currentLanguage)?.name ?? 'English';
  }

  changeLanguage(language: string): void {

      this.languageService.changeLanguage(language);

  }
}
