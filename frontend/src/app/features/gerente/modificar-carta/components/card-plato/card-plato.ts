import { Component, output, input, ChangeDetectionStrategy } from '@angular/core';
import { Plato } from '../../../../../core/models/domain/plato';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';
import { ArsCurrencyPipe } from '../../../../../shared/pipes/ars-currency.pipe';

@Component({
  selector: 'app-card-plato',
  standalone: true,
  imports: [ToggleComponent, ArsCurrencyPipe],
  templateUrl: './card-plato.html',
  styleUrls: ['./card-plato.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CardPlatoComponent {
  
  plato = input.required<Plato>();
  layoutMode = input<'grid' | 'list'>('grid');
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
