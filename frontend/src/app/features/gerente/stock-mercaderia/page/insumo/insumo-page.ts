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
import { Insumo, LoteInsumo } from '../../../../../core/models/domain/insumo';
import { CrearInsumo } from '../../../../../core/models/domain/insumo';

type EstadoStockFiltro = 'todos' | 'criticos' | 'bajos' | 'ok';

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
  estadoFiltro = signal<EstadoStockFiltro>('todos');
  
  tabSeleccionada = signal<'productos' | 'bodegas' | 'lotes'>('productos');
  productoEditandoId = signal<number | null>(null);
  
  tituloModal= computed(() => {
    return this.productoEditandoId() ? 'Editar Insumo' : 'Nuevo Insumo'
  })
  modalAbierto = signal<boolean>(false);



  categoriasFiltro = computed(() =>{
    const nombres = this.state.categoriasInsumos().map(c => c.descripcion);
    return [...nombres];
  })

  totalProductos = computed(() => this.state.productos().length);

  productosCriticos = computed(() =>
    this.state.productos().filter(p => p.stockActual < p.stockMinimo).length
  );

  productosBajos = computed(() =>
    this.state.productos().filter(p => p.stockActual >= p.stockMinimo && p.stockActual < p.stockMinimo * 2).length
  );

  productosOk = computed(() =>
    this.state.productos().filter(p => p.stockActual >= p.stockMinimo * 2).length
  );

  filtrosActivos = computed(() => {
    let total = 0;
    if (this.termino().trim()) total++;
    if (this.categoria() !== 'Categorías') total++;
    if (this.estadoFiltro() !== 'todos') total++;
    if (this.tabSeleccionada() === 'bodegas' && this.bodegaSeleccionadaId()) total++;
    return total;
  });

  contextoListado = computed(() => {
    if (this.tabSeleccionada() === 'bodegas') {
      return this.nombreBodegaSeleccionada() || 'Elegí una bodega';
    }
    if (this.tabSeleccionada() === 'lotes') return 'Lotes';
    if (this.estadoFiltro() === 'criticos') return 'Productos críticos';
    if (this.estadoFiltro() === 'bajos') return 'Productos bajos';
    if (this.estadoFiltro() === 'ok') return 'Productos ok';
    return 'Todos los productos';
  });

 productoSeleccionado = computed(() => {
  const id = this.productoEditandoId();
  if (!id) return null;
  return this.state.productos().find(p => p.id === id) || null;
});
 
  
  productosFiltrados = computed(() => {
   let listaBase: Insumo[] = [];

    if (this.tabSeleccionada() === 'bodegas') {
      const bId = this.bodegaSeleccionadaId();
      if (bId) {
        const bodega = this.bodegaState.bodegas().find(b => b.id === bId);
        listaBase = bodega?.insumos || [];
      } else {
        return [];
      }
    } else if (this.tabSeleccionada() === 'lotes') {
      return [];
    } else {
      listaBase = this.state.productos();
    }
    const busqueda = this.termino().toLowerCase();
    const cat = this.categoria();
    
    if (busqueda) {
      listaBase = listaBase.filter(p => p.nombre.toLowerCase().includes(busqueda));
    }
    if (cat && cat !== 'Categorías') {
      listaBase = listaBase.filter(p => p.categoriaIngrediente?.descripcion === cat);
    }
    listaBase = this.filtrarPorEstado(listaBase);
    
    return listaBase;
  })

  lotesVista = computed(() => {
    if (this.tabSeleccionada() !== 'lotes') return [];

    const busqueda = this.termino().toLowerCase();
    const cat = this.categoria();
    const productosPorId = new Map(this.state.productos().map(p => [p.id, p]));
    const bodegasPorId = new Map(this.bodegaState.bodegas().map(b => [b.id, b.nombre]));

    return this.state.lotes()
      .map(lote => ({
        lote,
        producto: productosPorId.get(lote.insumoId) || null,
        bodega: bodegasPorId.get(lote.bodegaId) || 'Sin bodega',
        unidad: productosPorId.get(lote.insumoId)?.unidadMedida.nombre || '',
        estadoClase: this.estadoLote(lote),
        estadoTexto: this.textoEstadoLote(lote)
      }))
      .filter(item => item.producto)
      .filter(item => {
        if (!busqueda) return true;
        return item.producto!.nombre.toLowerCase().includes(busqueda) ||
          item.lote.nombre.toLowerCase().includes(busqueda);
      })
      .filter(item => cat === 'Categorías' || item.producto!.categoriaIngrediente.descripcion === cat);
  });

  lotesCriticos = computed(() => this.lotesVista().filter(item => item.estadoClase === 'danger').length);

  bodegaSeleccionadaId = signal<number | null>(null);
  nombreBodegaSeleccionada = computed(() => {
    const id = this.bodegaSeleccionadaId();
    if(!id) return null;
    return this.bodegaState.bodegas().find(b => b.id === id)?.nombre || null;
  });
  seleccionarBodega(id: number) {
    this.bodegaState.cargarBodegasConInsumos();
    this.bodegaSeleccionadaId.set(id);
    this.tabSeleccionada.set('bodegas');
  }

  seleccionarProductos() {
    this.tabSeleccionada.set('productos');
    this.bodegaSeleccionadaId.set(null);
  }

  seleccionarEstado(estado: EstadoStockFiltro) {
    this.estadoFiltro.set(estado);
    if (this.tabSeleccionada() === 'lotes') {
      this.seleccionarProductos();
    }
  }

  seleccionarLotes() {
    this.state.cargarLotes();
    this.tabSeleccionada.set('lotes');
    this.bodegaSeleccionadaId.set(null);
  }

  limpiarFiltros() {
    this.termino.set('');
    this.categoria.set('Categorías');
    this.estadoFiltro.set('todos');
    if (this.tabSeleccionada() === 'bodegas' && !this.bodegaSeleccionadaId()) {
      this.tabSeleccionada.set('productos');
    }
  }

  private filtrarPorEstado(productos: Insumo[]): Insumo[] {
    const estado = this.estadoFiltro();
    if (estado === 'todos') return productos;

    return productos.filter((producto) => {
      const critico = producto.stockActual < producto.stockMinimo;
      const bajo = producto.stockActual >= producto.stockMinimo && producto.stockActual < producto.stockMinimo * 2;
      const ok = producto.stockActual >= producto.stockMinimo * 2;

      return (estado === 'criticos' && critico) ||
        (estado === 'bajos' && bajo) ||
        (estado === 'ok' && ok);
    });
  }



  ngOnInit() {
    this.state.cargarMercaderia();
    this.bodegaState.cargarBodegas();
    this.state.cargarCatalogos(); 
  }

  diasHastaVencimiento(lote: LoteInsumo): number {
    if (!lote.fechaVencimiento) return Number.POSITIVE_INFINITY;
    const hoy = new Date();
    hoy.setHours(0, 0, 0, 0);
    const vencimiento = new Date(`${lote.fechaVencimiento}T00:00:00`);
    return Math.ceil((vencimiento.getTime() - hoy.getTime()) / 86400000);
  }

  estadoLote(lote: LoteInsumo): 'danger' | 'warning' | 'success' {
    const dias = this.diasHastaVencimiento(lote);
    if (dias <= 7) return 'danger';
    if (dias <= 30) return 'warning';
    return 'success';
  }

  textoEstadoLote(lote: LoteInsumo): string {
    const dias = this.diasHastaVencimiento(lote);
    if (!Number.isFinite(dias)) return 'Sin fecha';
    if (dias < 0) return 'Vencido';
    if (dias === 0) return 'Vence hoy';
    if (dias <= 7) return `${dias} días`;
    if (dias <= 30) return 'Próximo';
    return 'Ok';
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
    guardarCambios(datosProducto: CrearInsumo, modal:Modal){
      this.state.guardarProducto(datosProducto);
      modal.cerrar();
      this.limpiarEstadoModal();
    }
  
}
