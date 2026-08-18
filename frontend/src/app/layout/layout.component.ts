import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';

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
export class LayoutComponent implements OnInit {

  sidenavOpenedSignal = signal(true);

  ngOnInit(): void {

    if (localStorage.getItem('theme') === 'dark') {
      document.documentElement.classList.add('dark-theme');
    }

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