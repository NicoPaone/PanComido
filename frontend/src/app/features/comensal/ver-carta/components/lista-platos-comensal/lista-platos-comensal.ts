import { Component, input, output , ChangeDetectionStrategy} from '@angular/core';
import { CardPlatoComensalComponent } from '../card-plato-comensal/card-plato-comensal';
import { ItemPedido } from '../../../../../core/models/domain/item-pedido';
import { CartaItem } from '../../../../../core/models/domain/carta-item';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-lista-platos-comensal',
  standalone: true,
  imports: [CardPlatoComensalComponent],
  templateUrl: './lista-platos-comensal.html',
})
export class ListaPlatosComensalComponent {
  

  platos = input.required<CartaItem[]>();
  agregarPedido = output<ItemPedido>();

  onAgregarPedido(item: ItemPedido): void {
    this.agregarPedido.emit(item);
  }
}
