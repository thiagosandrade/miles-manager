import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private _darkTheme = signal(false);
  darkTheme$ = this._darkTheme;

  toggleTheme(): void {
    this._darkTheme.update(current => !current);
    if(this._darkTheme()) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }

  setDarkTheme(value: boolean): void {
    this._darkTheme.set(value);
    if(value) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  }
}
