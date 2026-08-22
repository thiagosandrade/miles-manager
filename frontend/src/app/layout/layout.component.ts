import {
  Component,
  OnDestroy,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { BreakpointObserver } from '@angular/cdk/layout';
import { RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';

import { SidebarComponent } from './sidebar/sidebar.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { FooterComponent } from './footer/footer.component';

@Component({
  standalone: true,
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  imports: [
    CommonModule,
    SidebarComponent,
    ToolbarComponent,
    FooterComponent,
    RouterModule
  ]
})
export class LayoutComponent implements OnInit, OnDestroy {

  private readonly breakpointObserver =
    inject(BreakpointObserver);

  private breakpointSubscription?: Subscription;

  sidenavOpenedSignal = signal(true);

  ngOnInit(): void {

    if (localStorage.getItem('theme') === 'dark') {
      document.documentElement.classList.add('dark-theme');
    }

    this.breakpointSubscription =
      this.breakpointObserver
        .observe('(max-width: 768px)')
        .subscribe(result => {

          if (result.matches) {
            // Mobile
            this.sidenavOpenedSignal.set(false);
          } else {
            // Desktop
            this.sidenavOpenedSignal.set(true);
          }

        });

  }

  ngOnDestroy(): void {
    this.breakpointSubscription?.unsubscribe();
  }

  toggleSidenav(): void {

    this.sidenavOpenedSignal.update(
      opened => !opened
    );

  }

  closeSidenav(): void {

    this.sidenavOpenedSignal.set(false);

  }

}