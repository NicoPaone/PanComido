import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';

@Component({
  selector: 'app-card-plato',
  standalone: true,
  imports: [CommonModule, Boton, ToggleComponent],
  templateUrl: './card-plato.html',
  styleUrls: ['./card-plato.css'],
})
export class CardPlatoComponent {
  // Entrada: datos del plato (requerido)
  plato = input.required<Plato>();
  
  // Salida: evento cuando se toggle la visibilidad del plato
  toggleVisible = output<Plato>();

  /**
   * Emite el evento de toggle con el plato actual
   */
  onToggle() {
    this.toggleVisible.emit(this.plato());
  }
}

