import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  imports: [CommonModule, MatSidenavModule, MatListModule, RouterModule, MatIconModule],
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  @Input() opened = false;
  @Output() closeSidenav = new EventEmitter<void>();

  navigation = [
    { path: '/dashboard', label: 'Dashboard', icon: 'dashboard' },
    { path: '/users', label: 'Users', icon: 'person' },
    { path: '/programs', label: 'Programs', icon: 'assignment' },
    { path: '/program-balances', label: 'Program Balances', icon: 'account_balance_wallet' },
    { path: '/purchases', label: 'Purchases', icon: 'shopping_cart' },
    { path: '/transfers', label: 'Transfers', icon: 'swap_horiz' },
    { path: '/redemptions', label: 'Redemptions', icon: 'redeem' }
  ];

  onClose(): void {
    this.closeSidenav.emit();
  }
}
