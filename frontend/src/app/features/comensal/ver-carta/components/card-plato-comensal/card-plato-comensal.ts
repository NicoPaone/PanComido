import { Component, EventEmitter, input, Input, output, Output, signal , ChangeDetectionStrategy} from '@angular/core';

import { ItemPedido } from '../../../../../core/models/domain/item-pedido';
import { BotonComensal } from '../../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../../infra/mocks/configuracion-restaurante.mock-data';
import { CartaItem } from '../../../../../core/models/domain/carta-item';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';


 @Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-card-plato-comensal',
  standalone: true,
  imports: [BotonComensal, Boton],
  templateUrl: './card-plato-comensal.html',
  styleUrls: ['./card-plato-comensal.css'],
})
export class CardPlatoComensalComponent {

  plato = input.required<CartaItem>();
  agregarPedido = output<ItemPedido>();

  configuracion = configuracionRestauranteMock;

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