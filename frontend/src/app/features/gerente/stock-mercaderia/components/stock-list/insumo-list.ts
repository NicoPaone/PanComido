import { Component, input, output } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';
import { faCog, faSort } from '@fortawesome/free-solid-svg-icons';
import { Insumo } from '../../../../../core/models/insumos/insumo';

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
    if (item.stockActual <= item.stockMinimo * 0.5) return 'bg-critical';
    if (item.stockActual <= item.stockMinimo) return 'bg-warning';
    return 'bg-success';
  }
}
