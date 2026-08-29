import { Injectable, signal } from '@angular/core';

const THEME_KEY = 'theme';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  private readonly _darkTheme = signal(false);

  readonly isDarkTheme = this._darkTheme.asReadonly();

  constructor() {
    this.loadTheme();
  }

  toggleTheme(): void {
    this.setDarkTheme(!this._darkTheme());
  }

  setDarkTheme(value: boolean): void {
    this._darkTheme.set(value);

    document.documentElement.classList.toggle('dark-theme', value);

    localStorage.setItem(
      THEME_KEY,
      value ? 'dark' : 'light'
    );
  }

  private loadTheme(): void {
    const savedTheme = localStorage.getItem(THEME_KEY);

    this.setDarkTheme(savedTheme === 'dark');
  }
}