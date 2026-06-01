import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, ElementRef, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark } from '@fortawesome/free-solid-svg-icons';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { PedidoProveedor, EstadoPedidoProveedor, Proveedor } from '../../../../core/models/proveedor';
import { Router, RouterModule } from '@angular/router';
import { ProveedorListComponent } from '../components/proveedor-list/proveedor-list';
import { Insumo as ProductoStockMock } from '../../../../core/models/insumos/insumo';
import { VerProveedoresStateService } from '../services/ver-proveedores.state';

@Component({
  selector: 'app-ver-proveedores',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, FontAwesomeModule, Buscador, Boton, Dropdown, PageToolbar, ProveedorListComponent, RouterModule],
  templateUrl: './ver-proveedores.html',
  styleUrls: ['./ver-proveedores.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VerProveedoresComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly state = inject(VerProveedoresStateService);

  private readonly pedidoModalCard = viewChild<ElementRef<HTMLElement>>('pedidoModalCard');

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  termino = this.state.termino;
  proveedores = this.state.proveedores;
  productos = this.state.productos;
  proveedorSeleccionadoId = this.state.proveedorSeleccionadoId;
  panelModo = this.state.panelModo;
  observacionPedido = this.state.observacionPedido;
  mensajeAccion = this.state.mensajeAccion;
  
  productoTexto = this.state.productoTexto;
  productoSeleccionadoId = this.state.productoSeleccionadoId;
  cantidadProducto = this.state.cantidadProducto;
  precioProductoManual = this.state.precioProductoManual;
  
  pedidoItems = this.state.pedidoItems;
  pedidoHistorialSeleccionado = this.state.pedidoHistorialSeleccionado;
  
  proveedoresFiltrados = this.state.proveedoresFiltrados;
  productosFiltrados = this.state.productosFiltrados;
  proveedorSeleccionado = this.state.proveedorSeleccionado;
  productoBaseActual = this.state.productoBaseActual;

  faCheck = faCheck;
  faXmark = faXmark;

  ngOnInit(): void {
    this.state.cargarDatos();

    const navState = history.state as { created?: boolean; message?: string } | undefined;
    if (navState?.created) {
      this.state.mensajeAccion.set(navState.message ?? 'Proveedor creado correctamente');
      setTimeout(() => this.state.mensajeAccion.set(null), 3500);
    }
  }

  abrirNuevoProveedorRoute(): void {
    this.router.navigate(['/staff', 'gerente', 'nuevo-proveedor']);
  }

  irAPedidoSugeridoIA(proveedor: Proveedor): void {
    this.router.navigate(['/staff', 'gerente', 'pedido-sugerido-ia', proveedor.id]);
  }

  seleccionarProveedor(proveedor: Proveedor): void {
    this.state.seleccionarProveedor(proveedor.id);
  }

  abrirPedido(proveedor: Proveedor): void {
    this.state.abrirPedido(proveedor.id);
  }

  abrirHistorial(proveedor: Proveedor): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores', proveedor.id, 'historial']);
  }

  abrirDetallePedido(pedido: PedidoProveedor): void {
    this.state.abrirDetallePedido(pedido);
    setTimeout(() => this.focusModal(), 50);
  }

  cerrarDetallePedido(): void {
    this.state.cerrarDetallePedido();
  }

  private focusModal(): void {
    try {
      const el = this.pedidoModalCard()?.nativeElement;
      if (el) {
        el.focus({ preventScroll: true });
      }
    } catch (e) {
      // Ignorar errores de foco
    }
  }

  cambiarProveedorDesdePedido(proveedor: Proveedor, dropdown?: Dropdown): void {
    this.state.limpiarPedido();
    this.state.seleccionarProveedor(proveedor.id);
    this.state.panelModo.set('pedido');
    dropdown?.cerrar();
  }

  seleccionarProducto(producto: ProductoStockMock): void {
    this.state.seleccionarProducto(producto);
  }

  onProductoTextoChange(valor: string): void {
    this.state.onProductoTextoChange(valor);
  }

  agregarItemPedido(): void {
    this.state.agregarItemPedido();
  }

  actualizarCantidadItem(itemId: string | number, cantidad: number | null): void {
    this.state.actualizarCantidadItem(itemId, cantidad);
  }

  eliminarItemPedido(itemId: string | number): void {
    this.state.eliminarItemPedido(itemId);
  }

  limpiarPedido(): void {
    this.state.limpiarPedido();
  }

  enviarPedido(): void {
    this.state.enviarPedido();
  }

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':
        return 'estado-recibido';
      case 'Confirmado':
        return 'estado-confirmado';
      case 'Cancelado':
        return 'estado-cancelado';
      default:
        return 'estado-pendiente';
    }
  }

  get totalPedidosSeleccionado(): number {
    return this.state.totalPedidosSeleccionado();
  }

  get proveedorSeleccionadoPedido(): Proveedor | null {
    return this.state.proveedorSeleccionado();
  }

  get montoEstimado(): number {
    return this.state.montoEstimado();
  }

  get productoSeleccionadoActual(): ProductoStockMock | null {
    return this.state.productoSeleccionadoActual();
  }

  get cantidadPasoProducto(): number {
    return this.state.cantidadPasoProducto();
  }

  get cantidadMinimaProducto(): number {
    return this.state.cantidadMinimaProducto();
  }

  get cantidadPlaceholderProducto(): string {
    return this.state.cantidadPlaceholderProducto();
  }
}