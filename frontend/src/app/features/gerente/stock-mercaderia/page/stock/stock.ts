import { Component, computed, signal } from '@angular/core';
import { StockList } from '../../components/stock-list/stock-list';
import { CommonModule } from '@angular/common';
import { ProductoStockMock, PRODUCTOS_STOCK_MOCK } from '../../../../../core/model/producto-stock-mock';
import { Boton } from "../../../../../shared/ui/botones/boton/boton";
import { PageToolbar } from "../../../../../shared/ui/page-toolbar/page-toolbar";
import { Buscador } from "../../../../../shared/ui/buscador/buscador";
import { Dropdown } from '../../../../../shared/ui/dropdown/dropdown';

@Component({
  selector: 'app-stock',
  imports: [StockList, CommonModule, Boton, PageToolbar, Buscador,Dropdown],
  templateUrl: './stock.html',
  styleUrl: './stock.css',
})
export class Stock {
  productosMock = signal<ProductoStockMock[]>(PRODUCTOS_STOCK_MOCK);
  termino = signal<string>('');
  categoria = signal<string>('Categorías');
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');

  productosFiltrados = computed(() => {
    let lista = this.productosMock();
    const busqueda = this.termino().toLowerCase();
    const cat = this.categoria();

    if(busqueda){
      lista = lista.filter(p => p.nombre.toLowerCase().includes(busqueda));
    }
    if(cat){
      //productos todavia no tiene categoria.
    }
    return lista;
  })

  agregarNuevoProducto(){

    //TODO
    console.log("Abrir modal para agregar nuevo producto");
  }
}
