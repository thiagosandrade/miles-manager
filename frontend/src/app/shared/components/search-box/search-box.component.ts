import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-search-box',
  imports: [CommonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './search-box.component.html',
  styleUrls: ['./search-box.component.css']
})
export class SearchBoxComponent {
  private searchSubject = new Subject<string>();

  @Output() search = new EventEmitter<string>();

  constructor() {
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(value => this.search.emit(value));
  }

  onInput(value: string): void {
    this.searchSubject.next(value);
  }
}
