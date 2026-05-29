import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { InsumoList } from '../../components/stock-list/insumo-list';
import { CommonModule } from '@angular/common';
import { Boton } from "../../../../../shared/ui/botones/boton/boton";
import { PageToolbar } from "../../../../../shared/ui/page-toolbar/page-toolbar";
import { Buscador } from "../../../../../shared/ui/buscador/buscador";
import { Dropdown } from '../../../../../shared/ui/dropdown/dropdown';
import { Modal } from "../../../../../shared/ui/modal/modal";
import { StockMercaderiaState } from '../../services/stock-mercaderia-state';
import { CategoriaIngrediente } from '../../../../../core/models/producto-stock';

@Component({
  selector: 'app-insumo',
  standalone: true,
  imports: [InsumoList, CommonModule, Boton, PageToolbar, Buscador, Dropdown, Modal],
  templateUrl: './insumo-page.html',
  styleUrl: './insumo-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InsumoPage {

  protected state = inject(StockMercaderiaState);

  
  
  termino = signal<string>('');
  categoria = signal<string>('Categorías');
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');
  productoEditandoId = signal<number | null>(null);
  
  tituloModal= computed(() => {
    return this.productoEditandoId() ? 'Editar Producto' : 'Nuevo Insumo'
  })
  productoSeleccionado = computed(() => {
    const id = this.productoEditandoId();
    return this.state.productos().find(p => p.id === id) || null;
  })

  
  
  productosFiltrados = computed(() => {
    let lista = this.state.productos();
    const busqueda = this.termino().toLowerCase();
    const cat = this.categoria();
    
    if(busqueda){
      lista = lista.filter(p => p.nombre.toLowerCase().includes(busqueda));
    }
    if(cat && cat !== 'Categorías'){
      
      lista = lista.filter(p => p.categoria === cat);
      
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
    limpiarEstadoModal() { 
      this.productoEditandoId.set(null);
    }
    guardarCambios(datosProducto: any, modal:Modal){
      this.state.guardarProducto(datosProducto);
      modal.cerrar();
      this.limpiarEstadoModal();
    }
  
}
