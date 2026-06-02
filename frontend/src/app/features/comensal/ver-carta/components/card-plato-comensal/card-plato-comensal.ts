import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../../app/shared/ui/botones/boton/boton';


@Component({
  selector: 'app-card-plato-comensal',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './card-plato-comensal.html',
  styleUrls: ['./card-plato-comensal.css'],
})
export class CardPlatoComensalComponent {

  @Input({ required: true }) plato!: Plato;

  @Output() agregarPedido = new EventEmitter<Plato>();

  agregar() {
    this.agregarPedido.emit(this.plato);
  }

}