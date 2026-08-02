import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.css'
})
export class AppShellComponent {
  protected readonly navItems = [
    { label: 'Dashboard', path: '/dashboard', hint: 'Overview' },
    { label: 'Users', path: '/users', hint: 'Access control' },
    { label: 'Programs', path: '/programs', hint: 'Coming soon' },
    { label: 'Reports', path: '/reports', hint: 'Coming soon' },
    { label: 'Settings', path: '/settings', hint: 'Coming soon' }
  ];
}
