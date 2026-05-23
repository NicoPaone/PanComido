import { Component, computed, signal } from '@angular/core';
import { StockList } from '../../components/stock-list/stock-list';
import { StockToolbar } from '../../components/stock-toolbar/stock-toolbar';
import { CommonModule } from '@angular/common';
import { ProductoStockMock, PRODUCTOS_STOCK_MOCK } from '../../../../../core/model/producto-stock-mock';
import { Boton } from "../../../../../shared/ui/boton/boton";

@Component({
  selector: 'app-stock',
  imports: [StockList, StockToolbar, CommonModule, Boton],
  templateUrl: './stock.html',
  styleUrl: './stock.css',
})
export class Stock {
  productosMock = signal<ProductoStockMock[]>(PRODUCTOS_STOCK_MOCK);
  termino = signal<string>('');
  categoria = signal<string>('');
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');

  productosFiltrados = computed(() => {
    let lista = this.productosMock();
    const busqueda = this.termino().toLowerCase();

    if(busqueda){
      lista = lista.filter(p => p.nombre.toLowerCase().includes(busqueda));
    }
    return lista;
  })
}
