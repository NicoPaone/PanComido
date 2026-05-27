import { Component, Input, Output, EventEmitter, output, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';

@Component({
  selector: 'app-card-plato',
  standalone: true,
  imports: [CommonModule, Boton, ToggleComponent],
  templateUrl: './card-plato.html',
  styleUrls: ['./card-plato.css'],
})
export class CardPlatoComponent {
  
  plato = input.required<Plato>();
  toggleVisible = output<Plato>();
  editar = output<number>();
  
  onToggle() {
    this.toggleVisible.emit(this.plato());
  }
}
