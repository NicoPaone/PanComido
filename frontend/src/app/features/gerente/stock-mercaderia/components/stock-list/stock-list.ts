import { Component, input, output } from '@angular/core';
import { ProductoStockMock } from '../../../../../core/model/producto-stock-mock';

@Component({
  selector: 'app-stock-list',
  imports: [],
  templateUrl: './stock-list.html',
  styleUrl: './stock-list.css',
})
export class StockList {

  productos = input.required<ProductoStockMock[]>();

  editarProducto = output<string>();
}
