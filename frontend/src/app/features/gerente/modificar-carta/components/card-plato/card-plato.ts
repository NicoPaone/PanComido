import { Component, output, input } from '@angular/core';
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
  isExploding = input<boolean>(false);
  toggleVisible = output<Plato>();
  editPlato = output<Plato>();
  deletePlato = output<Plato>();
  
  onToggle() {
    this.toggleVisible.emit(this.plato());
  }

  onEdit() {
    this.editPlato.emit(this.plato());
  }

  onDelete() {
    this.deletePlato.emit(this.plato());
  }
}
