import { Component, Output, Input, EventEmitter, inject, Injectable, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent
} from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';

@Injectable()
class PaginationIntl extends MatPaginatorIntl implements OnDestroy {
  private readonly translate = inject(TranslateService);
  private readonly destroy$ = new Subject<void>();

  constructor() {
    super();
    this.updateLabels();

    this.translate.onLangChange
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.updateLabels());
  }

  private updateLabels(): void {
    this.itemsPerPageLabel = this.translate.instant('common.pagination.itemsPerPage');
    this.nextPageLabel = this.translate.instant('common.pagination.nextPage');
    this.previousPageLabel = this.translate.instant('common.pagination.previousPage');
    this.firstPageLabel = this.translate.instant('common.pagination.firstPage');
    this.lastPageLabel = this.translate.instant('common.pagination.lastPage');

    this.getRangeLabel = (page: number, pageSize: number, length: number) => {
      if (length === 0 || pageSize === 0) {
        return `0 ${this.translate.instant('common.pagination.of')} 0`;
      }

      const startIndex = page * pageSize;
      const endIndex = Math.min(startIndex + pageSize, length);

      return `${startIndex + 1} – ${endIndex} ${this.translate.instant('common.pagination.of')} ${length}`;
    };

    this.changes.next();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

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
