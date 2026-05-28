import { Component, output, input, ChangeDetectionStrategy } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';

@Component({
  selector: 'app-card-plato',
  standalone: true,
  imports: [DecimalPipe, Boton, ToggleComponent],
  templateUrl: './card-plato.html',
  styleUrls: ['./card-plato.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CardPlatoComponent {
  
  plato = input.required<Plato>();
  isExploding = input<boolean>(false);
  toggleVisible = output<Plato>();
  editPlato = output<Plato>();
  deletePlato = output<Plato>();
  toggleRecomendado = output<Plato>();
  
  onToggle() {
    this.toggleVisible.emit(this.plato());
  }

  onToggleRecomendado() {
    this.toggleRecomendado.emit(this.plato());
  }

  onEdit() {
    this.editPlato.emit(this.plato());
  }

  onDelete() {
    this.deletePlato.emit(this.plato());
  }
}
