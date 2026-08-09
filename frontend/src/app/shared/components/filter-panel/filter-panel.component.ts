import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';

@Component({
  standalone: true,
  selector: 'app-filter-panel',
  imports: [MatExpansionModule],
  templateUrl: './filter-panel.component.html',
  styleUrls: ['./filter-panel.component.css']
})
export class FilterPanelComponent {
  @Input() title = 'Filters';
  @Output() toggle = new EventEmitter<boolean>();
}
