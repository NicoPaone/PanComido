import { CommonModule } from '@angular/common';
import { Component, ChangeDetectionStrategy, Input } from '@angular/core';

/**
 * AppNotice component – a lightweight banner/notification used for informational messages.
 * Supports three visual variants: info, success, error.
 */
@Component({
  selector: 'app-notice',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app-notice.html',
  styleUrls: ['./app-notice.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppNotice {
  /** Message text to display */
  @Input() message: string = '';
  /** Variant controlling colour palette */
  @Input() type: 'info' | 'success' | 'error' = 'info';
}
