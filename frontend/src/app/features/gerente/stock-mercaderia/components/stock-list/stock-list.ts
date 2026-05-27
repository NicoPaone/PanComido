import { Component, input, output } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CommonModule } from '@angular/common';
import { faCog, faSort } from '@fortawesome/free-solid-svg-icons';
import { ProductoStock } from '../../../../../core/model/producto-stock';

@Component({
  selector: 'app-stock-list',
  imports: [CommonModule,FontAwesomeModule],
  templateUrl: './stock-list.html',
  styleUrl: './stock-list.css',
})
export class StockList {

  productos = input.required<ProductoStock[]>();
  onAction = output<number>();
  faCog = faCog;
  faSort = faSort;
  editar = output<number>();
 getBadgeClass(item: ProductoStock): string {
    if (item.stock <= item.stockMinimo * 0.5) return 'bg-critical';
    if (item.stock <= item.stockMinimo) return 'bg-warning';
    return 'bg-success';
  }
}
