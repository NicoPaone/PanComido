import { Component, input, output } from '@angular/core';
import { Plato } from '../../../../../core/models/plato';
import { CardPlatoComponent } from '../card-plato/card-plato';

@Component({
  selector: 'app-lista-platos',
  standalone: true,
  imports: [CardPlatoComponent],
  templateUrl: './lista-platos.html',
})
export class ListaPlatosComponent {
  platos = input<Plato[]>([]);
  explodingId = input<number | null>(null);
  toggleVisible = output<Plato>();
  editPlato = output<Plato>();
  deletePlato = output<Plato>();

  onToggleVisible(plato: Plato) {
    this.toggleVisible.emit(plato);
  }

  onEditPlato(plato: Plato) {
    this.editPlato.emit(plato);
  }

  onDeletePlato(plato: Plato) {
    this.deletePlato.emit(plato);
  }
}
