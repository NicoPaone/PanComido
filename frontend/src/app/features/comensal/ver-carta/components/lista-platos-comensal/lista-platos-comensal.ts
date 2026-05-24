import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { CardPlatoComensalComponent } from '../card-plato-comensal/card-plato-comensal';

@Component({
  selector: 'app-lista-platos-comensal',
  standalone: true,
  imports: [CommonModule, CardPlatoComensalComponent],
  templateUrl: './lista-platos-comensal.html',
})
export class ListaPlatosComensalComponent {

  @Input() platos: Plato[] = [];

  @Output() agregarPedido = new EventEmitter<Plato>();

  onAgregarPedido(plato: Plato) {
    this.agregarPedido.emit(plato);
  }

}