import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { CardPlatoComponent } from '../card-plato/card-plato';

@Component({
  selector: 'app-lista-platos',
  standalone: true,
  imports: [CommonModule, CardPlatoComponent],
  templateUrl: './lista-platos.html',
})
export class ListaPlatosComponent {
  // Entrada: listado de platos a mostrar
  platos = input<Plato[]>([]);
  
  // Salida: evento cuando se toggle la visibilidad de un plato
  toggleVisible = output<Plato>();

  /**
   * Forward del evento del hijo (card-plato)
   */
  onToggleVisible(plato: Plato) {
    this.toggleVisible.emit(plato);
  }
}
