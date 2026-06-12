import { CommonModule } from '@angular/common';
import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

/**
 * Pill chip component used for status badges and toolbar actions.
 * Supports variant styling (primary, secondary, success, danger).
 */
@Component({
  selector: 'app-pill-chip',
  standalone: true,
  imports: [CommonModule],
  template: `<span class="pill-chip" [ngClass]="variant">{{ label }}<ng-container *ngIf="count !== null"> ({{ count }})</ng-container></span>`,
  styleUrls: ['./pill-chip.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PillChip {
  @Input() label: string = '';
  @Input() variant: 'primary' | 'secondary' | 'success' | 'danger' = 'primary';
  @Input() count: number | null = null;
}
