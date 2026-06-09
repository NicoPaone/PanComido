import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-price-note',
  standalone: true,
  templateUrl: './price-note.html',
  styleUrl: './price-note.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PriceNoteComponent {
  text = input.required<string>();
  compact = input(false);
}
