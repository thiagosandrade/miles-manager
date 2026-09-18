import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { Sort } from '@angular/material/sort';

import { AppLoadingSpinnerComponent } from '../app-loading-spinner/app-loading-spinner.component';
import { PaginationComponent } from '../pagination/pagination.component';

@Component({
  selector: 'app-table',
  standalone: true,
  templateUrl: './app-table.component.html',
  styleUrls: ['./app-table.component.css'],
  imports: [
    CommonModule,
    MatPaginatorModule,
    MatIconModule,
    AppLoadingSpinnerComponent,
    PaginationComponent
  ]
})
export class AppTableComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() loading = false;
  @Input() dataSource: unknown[] = [];
  @Input() showToolbar = false;
  @Input() emptyIcon = 'inbox';
  @Input() emptyTitle = 'No records found';
  @Input() emptyDescription = '';
  @Input() showPaginator = false;
  @Input() length = 0;
  @Input() pageSize = 5;
  @Input() pageSizeOptions = [5, 10, 25, 50];

  @Output() page = new EventEmitter<PageEvent>();
  @Output() sortChange = new EventEmitter<Sort>();

  private activeSort = '';
  private activeDirection: 'asc' | 'desc' | '' = '';

  @HostListener('click', ['$event'])
  onTableClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const header = target.closest('th[mat-header-cell]') as HTMLElement | null;

    if (!header) return;

    const columnClass = Array.from(header.classList)
      .find(value => value.startsWith('mat-column-'));
    const active = columnClass?.replace('mat-column-', '');

    if (!active || active === 'actions') return;

    if (this.activeSort !== active) {
      this.activeSort = active;
      this.activeDirection = 'asc';
    } else if (this.activeDirection === 'asc') {
      this.activeDirection = 'desc';
    } else {
      this.activeDirection = '';
    }

    const headers = (event.currentTarget as HTMLElement)
      .querySelectorAll('th[mat-header-cell]');

    headers.forEach(item => item.classList.remove('sort-asc', 'sort-desc'));
    header.classList.toggle('sort-asc', this.activeDirection === 'asc');
    header.classList.toggle('sort-desc', this.activeDirection === 'desc');

    this.sortChange.emit({
      active: this.activeSort,
      direction: this.activeDirection,
    });
  }

  onPageChange(event: PageEvent): void {
    this.page.emit(event);
  }
}
