import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  MatPaginatorModule,
  PageEvent
} from '@angular/material/paginator';

import { MatIconModule } from '@angular/material/icon';

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

  onPageChange(event: PageEvent): void {
    this.page.emit(event);
  }
}