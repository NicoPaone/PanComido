import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';

@Component({
  selector: 'app-card-plato',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './card-plato.html',
  styleUrls: ['./card-plato.css'],
})
export class CardPlatoComponent {
  @Input({ required: true }) plato!: Plato;
  @Output() toggleVisible = new EventEmitter<Plato>();

  onToggle() {
    this.toggleVisible.emit(this.plato);
  }
}
