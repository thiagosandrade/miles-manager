import { Component, Output, Input, EventEmitter, inject, Injectable, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent
} from '@angular/material/paginator';
import { PaginationIntl } from './pagination-translation.service';

@Component({
  standalone: true,
  selector: 'app-pagination',
  imports: [CommonModule, MatPaginatorModule],
  providers: [
    {
      provide: MatPaginatorIntl,
      useClass: PaginationIntl
    }
  ],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent {
  @Input() length = 0;
  @Input() pageSize = 10;
  @Input() pageSizeOptions: number[] = [5, 10, 25];
  @Input() showPaginator = true;
  @Input() dataSource: unknown[] = [];

  @Output() page = new EventEmitter<PageEvent>();

  onPageChange(event: PageEvent): void {
    this.page.emit(event);
  }
}
