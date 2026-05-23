import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-toggle',
  standalone: true,
  templateUrl: './toggle.html',
  styleUrls: ['./toggle.css']
})
export class ToggleComponent {
  @Input() active: boolean = false;
  @Input() label: string = 'Visible en carta';
  @Output() toggled = new EventEmitter<void>();

  onToggle() {
    this.toggled.emit();
  }
}
