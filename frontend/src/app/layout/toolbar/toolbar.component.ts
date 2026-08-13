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
  private userApi = inject(UserApiService);
  private authApi = inject(AuthApiService);
  private tokenStorage = inject(TokenStorageService);
  private router = inject(Router);
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

    try {
          this.userApi.getUser(id).subscribe(result => {
            this.userName.set(result.fullName);
            this.userEmail.set(result.email);
        });


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
