import { Component, EventEmitter, input, Input, output, Output, signal , ChangeDetectionStrategy} from '@angular/core';

import { ItemPedido } from '../../../../../core/models/item-pedido';
import { BotonComensal } from '../../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { CartaItem } from '../../../../../core/models/carta-item';



 @Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-card-plato-comensal',
  standalone: true,
  imports: [BotonComensal],
  templateUrl: './card-plato-comensal.html',
  styleUrls: ['./card-plato-comensal.css'],
})
export class CardPlatoComensalComponent {

  plato = input.required<CartaItem>();
  agregarPedido = output<ItemPedido>();


  cantidad = signal(1);

  incrementar(): void {
    this.cantidad.update(c => c + 1);
  }

  decrementar(): void {
    if (this.cantidad() > 1) {
      this.cantidad.update(c => c - 1);
    }
  }

  agregar(): void {
    this.agregarPedido.emit({
      plato: this.plato(),
      cantidad: this.cantidad(),
    });
  }

}