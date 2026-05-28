import { Component, input, output } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';
import { faCog, faSort } from '@fortawesome/free-solid-svg-icons';
import { Insumo } from '../../../../../core/models/producto-stock';

@Component({
  selector: 'app-insumo-list',
  imports: [CommonModule,FontAwesomeModule],
  templateUrl: './insumo-list.html',
  styleUrl: './insumo-list.css',
})
export class InsumoList {

  productos = input.required<Insumo[]>();
  onAction = output<number>();
  faCog = faCog;
  faSort = faSort;
  editar = output<number>();
 getBadgeClass(item: Insumo): string {
    if (item.stock <= item.stockMinimo * 0.5) return 'bg-critical';
    if (item.stock <= item.stockMinimo) return 'bg-warning';
    return 'bg-success';
  }
}
