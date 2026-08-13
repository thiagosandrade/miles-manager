import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  standalone: true,
  selector: 'app-sidebar',
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    TranslatePipe
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  @Input() opened = false;
  @Output() closeSidenav = new EventEmitter<void>();

  navigation = [
    {
      path: '/dashboard',
      label: 'sidebar.dashboard',
      icon: 'dashboard'
    },
    {
      path: '/users',
      label: 'sidebar.users',
      icon: 'person'
    },
    {
      path: '/programs',
      label: 'sidebar.programs',
      icon: 'assignment'
    },
    {
      path: '/program-balances',
      label: 'sidebar.programBalances',
      icon: 'account_balance_wallet'
    },
    {
      path: '/purchases',
      label: 'sidebar.purchases',
      icon: 'shopping_cart'
    },
    {
      path: '/transfers',
      label: 'sidebar.transfers',
      icon: 'swap_horiz'
    },
    {
      path: '/redemptions',
      label: 'sidebar.redemptions',
      icon: 'redeem'
    }
  ];

  onClose(): void {
    this.closeSidenav.emit();
  }
}