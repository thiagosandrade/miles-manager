import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { SidebarComponent } from './sidebar/sidebar.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { FooterComponent } from './footer/footer.component';
import { RouterModule } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css'],
  imports: [CommonModule, SidebarComponent, ToolbarComponent, FooterComponent, RouterModule]
})
export class LayoutComponent {
  private breakpointObserver = new BreakpointObserver();
  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(map(result => result.matches));

  sidenavOpenedSignal = signal(true);

  toggleSidenav(): void {
    this.sidenavOpenedSignal.update(opened => !opened);
  }
}

// Note: This uses CDK BreakpointObserver to detect handset and toggle sidenav.
// It links toolbar menu button to toggle the sidebar opening state.
