import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Subscription } from 'rxjs';

import { TokenStorageService } from '../../core/services/token-storage.service';

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
export class SidebarComponent implements OnInit, OnDestroy {

  @Input() opened = false;

  @Output()
  closeSidenav = new EventEmitter<void>();

  private readonly storage = inject(TokenStorageService);
  private readonly breakpointObserver = inject(BreakpointObserver);

  private breakpointSubscription?: Subscription;

  readonly isAdministrator = this.storage.isAdministrator();

  isMobile = false;

  sidenavMode: 'side' | 'over' = 'side';

  navigation = [
    {
      path: '/dashboard',
      label: 'sidebar.dashboard',
      icon: 'dashboard'
    },
    {
      path: '/users',
      label: 'sidebar.users',
      icon: 'person',
      adminOnly: true
    },
    {
      path: '/user-roles',
      label: 'sidebar.user-roles',
      icon: 'admin_panel_settings',
      adminOnly: true
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
    },
    {
      path: '/calculator',
      label: 'sidebar.calculator',
      icon: 'calculate'
    }
  ];

  get visibleNavigation() {
    return this.navigation.filter(item =>
      !item.adminOnly || this.isAdministrator
    );
  }

  ngOnInit(): void {

    this.breakpointSubscription =
      this.breakpointObserver
        .observe('(max-width: 768px)')
        .subscribe(result => {

          this.isMobile = result.matches;

          this.sidenavMode = this.isMobile
            ? 'over'
            : 'side';

        });

  }

  ngOnDestroy(): void {
    this.breakpointSubscription?.unsubscribe();
  }

  onClose(): void {
    this.closeSidenav.emit();
  }

  onNavigationClick(): void {
    if (this.isMobile) {
      this.closeSidenav.emit();
    }
  }
}