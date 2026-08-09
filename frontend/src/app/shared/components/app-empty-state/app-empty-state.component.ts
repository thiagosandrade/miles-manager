import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-empty-state',
  imports: [CommonModule],
  templateUrl: './app-empty-state.component.html',
  styleUrls: ['./app-empty-state.component.css']
})
export class AppEmptyStateComponent {
  @Input() message = 'No data available.';
}
