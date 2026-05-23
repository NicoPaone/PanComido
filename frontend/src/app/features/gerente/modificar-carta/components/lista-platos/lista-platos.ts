import { Component, Input, Output, EventEmitter } from '@angular/core';
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
  @Input() platos: Plato[] = [];
  @Output() toggleVisible = new EventEmitter<Plato>();

  onToggleVisible(plato: Plato) {
    this.toggleVisible.emit(plato);
  }
}
