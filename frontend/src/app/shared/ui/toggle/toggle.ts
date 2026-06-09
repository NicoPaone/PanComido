import { Component, input, output , ChangeDetectionStrategy} from '@angular/core';

/**
 * Componente Toggle - Control de activación/desactivación
 * 
 * Uso moderno con Signals (Angular 21+):
 * - `active` es un Signal de entrada (read-only)
 * - `label` es un Signal de entrada con valor por defecto
 * - `toggled` es un Signal de salida que emite void
 * - No requiere @Input/@Output/@EventEmitter
 */
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-toggle',
  standalone: true,
  templateUrl: './toggle.html',
  styleUrls: ['./toggle.css']
})
export class ToggleComponent {
  // Entrada: estado del toggle
  active = input<boolean>(false);
  
  // Entrada: etiqueta visible al usuario
  label = input<string>('Visible en carta');
  
  // Salida: evento cuando el usuario clickea el toggle
  toggled = output<void>();

  /**
   * Maneja el click del usuario.
   * Emite el evento toggled sin datos (void).
   */
  onToggle() {
    this.toggled.emit();
  }
}
