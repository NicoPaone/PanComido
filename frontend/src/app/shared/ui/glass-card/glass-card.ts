import { CommonModule } from '@angular/common';
import { Component, ChangeDetectionStrategy, Input } from '@angular/core';

/**
 * Glass‑card component that provides a translucent background with backdrop blur.
 * Use it as a container for any content that should appear on a glass‑styled surface.
 */
@Component({
  selector: 'app-glass-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './glass-card.html',
  styleUrls: ['./glass-card.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GlassCard {
  /** Optional additional CSS classes */
  @Input() class = '';
}
