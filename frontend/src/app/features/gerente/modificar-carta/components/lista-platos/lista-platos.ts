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
  platos = input<Plato[]>([]);
  toggleVisible = output<Plato>();

  onToggleVisible(plato: Plato) {
    this.toggleVisible.emit(plato);
  }
}
