import { Component, input, output } from '@angular/core';
import { Plato } from '../../../../../core/models/plato';
import { CardPlatoComensalComponent } from '../card-plato-comensal/card-plato-comensal';
import { ItemPedido } from '../../../../../core/models/item-pedido';

@Component({
  selector: 'app-lista-platos-comensal',
  standalone: true,
  imports: [CardPlatoComensalComponent],
  templateUrl: './lista-platos-comensal.html',
})
export class ListaPlatosComensalComponent {
  platos = input.required<Plato[]>();
  agregarPedido = output<ItemPedido>();

  onAgregarPedido(item: ItemPedido): void {
    this.agregarPedido.emit(item);
  }
}
