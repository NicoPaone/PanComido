import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { StockList } from '../../components/stock-list/stock-list';
import { CommonModule } from '@angular/common';
import { ProductoStockMock, PRODUCTOS_STOCK_MOCK } from '../../../../../core/model/producto-stock-mock';
import { Boton } from "../../../../../shared/ui/botones/boton/boton";
import { PageToolbar } from "../../../../../shared/ui/page-toolbar/page-toolbar";
import { Buscador } from "../../../../../shared/ui/buscador/buscador";
import { Dropdown } from '../../../../../shared/ui/dropdown/dropdown';
import { Modal } from "../../../../../shared/ui/modal/modal";
import { StockMercaderiaState } from '../../services/stock-mercaderia-state';

@Component({
  selector: 'app-stock',
  imports: [StockList, CommonModule, Boton, PageToolbar, Buscador, Dropdown, Modal],
  templateUrl: './stock.html',
  styleUrl: './stock.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Stock {

  protected state = inject(StockMercaderiaState);

  productosMock = signal<ProductoStockMock[]>(PRODUCTOS_STOCK_MOCK);
  
  
  termino = signal<string>('');
  categoria = signal<string>('Categorías');
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');
  productoEditandoId = signal<number | null>(null);
  
  tituloModal= computed(() => {
    return this.productoEditandoId() ? 'Editar Producto' : 'Nuevo Producto'
  })
  productoSeleccionado = computed(() => {
    const id = this.productoEditandoId();
    return this.state.productos().find(p => p.id === id) || null;
  })
  
  productosFiltrados = computed(() => {
    let lista = this.productosMock();
    const busqueda = this.termino().toLowerCase();
    const cat = this.categoria();
    
    if(busqueda){
      lista = lista.filter(p => p.nombre.toLowerCase().includes(busqueda));
    }
    if(cat && cat !== 'Categorías'){
      
      lista = lista.filter(p => p.categoriaIngrediente === cat);
      
    }
    return lista;
  })
  ngOnInit() {
    this.state.cargarMercaderia();
  }
  
    abrirModalCrear(modal: Modal) { 
      this.productoEditandoId.set(null);
      modal.abrir();
    }
    abrirModalEditar(modal: Modal, id:number) {
      this.productoEditandoId.set(id);
      console.log("Abrir modal para editar producto con id:", id);
      modal.abrir();
    }
    cerrarYLimpiar(modal:Modal){
      modal.cerrar();
      this.productoEditandoId.set(null);
    }
    guardarCambios(datosProducto: any, modal:Modal){
      this.state.guardarProducto(datosProducto);
      this.cerrarYLimpiar(modal);
    }
  
}
