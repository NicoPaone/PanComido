import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../../app/shared/ui/botones/boton/boton';
import { ItemPedido } from '../../../../../core/models/item-pedido';
import { BotonComensal } from '../../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../../core/interceptors/handlers/configuracion-restaurante.mock';


@Component({
  selector: 'app-card-plato-comensal',
  standalone: true,
  imports: [CommonModule, Boton, BotonComensal],
  templateUrl: './card-plato-comensal.html',
  styleUrls: ['./card-plato-comensal.css'],
})
export class CardPlatoComensalComponent {

  @Input({ required: true }) plato!: Plato;

  @Output()
agregarPedido =
  new EventEmitter<ItemPedido>();

configuracion = configuracionRestauranteMock;

agregar() {

 this.agregarPedido.emit({
  plato: this.plato,
  cantidad: this.cantidad
});

}

  cantidad = 1;

incrementar() {

  this.cantidad++;

}

decrementar() {

  if (this.cantidad > 1) {

    this.cantidad--;

  }

}

}