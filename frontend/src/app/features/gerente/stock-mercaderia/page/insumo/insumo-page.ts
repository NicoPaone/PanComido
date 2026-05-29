import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { InsumoList } from '../../components/stock-list/insumo-list';
import { CommonModule } from '@angular/common';
import { Boton } from "../../../../../shared/ui/botones/boton/boton";
import { PageToolbar } from "../../../../../shared/ui/page-toolbar/page-toolbar";
import { Buscador } from "../../../../../shared/ui/buscador/buscador";
import { Dropdown } from '../../../../../shared/ui/dropdown/dropdown';
import { Modal } from "../../../../../shared/ui/modal/modal";
import { StockMercaderiaState } from '../../services/insumos/stock-mercaderia-state';
import { BodegaState } from '../../services/bodegas/bodega-state';
import { ProductoForm } from "../../components/producto-form/producto-form";

@Component({
  selector: 'app-insumo',
  standalone: true,
  imports: [InsumoList, CommonModule, Boton, PageToolbar, Buscador, Dropdown, Modal, ProductoForm],
  templateUrl: './insumo-page.html',
  styleUrl: './insumo-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InsumoPage {

  protected state = inject(StockMercaderiaState);
  protected bodegaState = inject(BodegaState);

  
  
  termino = signal<string>('');
  categoria = signal<string>('Categorías');
  
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');
  productoEditandoId = signal<number | null>(null);
  
  tituloModal= computed(() => {
    return this.productoEditandoId() ? 'Editar Insumo' : 'Nuevo Insumo'
  })
  modalAbierto = signal<boolean>(false);


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
    this.bodegaState.cargarBodegas();
  }
  
    abrirModalCrear(modal: Modal) { 
      this.productoEditandoId.set(null);
      this.modalAbierto.set(true);
      modal.abrir();
    }
    abrirModalEditar(modal: Modal, id:number) {
      this.productoEditandoId.set(id);
      this.modalAbierto.set(true);
      modal.abrir();
    }
    limpiarEstadoModal() { 
      this.productoEditandoId.set(null);
      this.modalAbierto.set(false);
    }
    guardarCambios(datosProducto: any, modal:Modal){
      this.state.guardarProducto(datosProducto);
      modal.cerrar();
      this.limpiarEstadoModal();
    }
  
}
