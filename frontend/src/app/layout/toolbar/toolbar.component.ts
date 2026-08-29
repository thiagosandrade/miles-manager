import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';

import { BreakpointObserver } from '@angular/cdk/layout';

import { Router } from '@angular/router';

import { Subscription } from 'rxjs';

import { AuthApiService } from '../../features/auth/auth-api.service';
import { TokenStorageService } from '../../core/services/token-storage.service';
import { LanguageService } from '../../core/services/language.service';
import { TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from '../../core/services/theme.service';

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
export class ToolbarComponent implements OnInit, OnDestroy {

  private readonly authApi = inject(AuthApiService);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly router = inject(Router);
  private readonly breakpointObserver = inject(BreakpointObserver);
  private readonly themeService = inject(ThemeService)

  private breakpointSubscription?: Subscription;

  @Output()
  sidenavToggle = new EventEmitter<void>();

  readonly user = this.tokenStorage.getUser();
  readonly isDarkTheme = this.themeService.isDarkTheme;

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

  isMobile = false;

  ngOnInit(): void {

    this.loadUserInfo();

    this.breakpointSubscription =
      this.breakpointObserver
        .observe('(max-width: 768px)')
        .subscribe(result => {

          this.isMobile = result.matches;

        });
  }

  ngOnDestroy(): void {
    this.breakpointSubscription?.unsubscribe();
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

  toggleSidenav(): void {
    this.sidenavToggle.emit();
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

    return this.languages.find(
      x => x.code === this.currentLanguage
    )?.name ?? 'English';

  }

  changeLanguage(language: string): void {

    this.languageService.changeLanguage(language);

  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}