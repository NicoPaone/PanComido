import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, ElementRef, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark } from '@fortawesome/free-solid-svg-icons';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { GlassCard } from '../../../../shared/ui/glass-card/glass-card';
import { PedidoProveedor, EstadoPedidoProveedor, Proveedor } from '../../../../core/models/domain/proveedor';
import { Router, RouterModule } from '@angular/router';
import { ProveedorListComponent } from '../components/proveedor-list/proveedor-list';
import { Insumo as ProductoStockMock } from '../../../../core/models/domain/insumo';
import { VerProveedoresState } from '../services/ver-proveedores.state';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { CategoriaInsumo } from '../../../../core/models/domain/categoria-insumo';
import { ArsCurrencyPipe } from '../../../../shared/pipes/ars-currency.pipe';
import { PriceNoteComponent } from '../../../../shared/ui/price-note/price-note';
import { buildSmartQuantityPresets, QuantityPreset } from '../../../../shared/utils/quantity-presets';

@Component({
  selector: 'app-ver-proveedores',
  standalone: true,
  imports: [DatePipe, FormsModule, FontAwesomeModule, Buscador, Boton, Dropdown, PageToolbar, GlassCard, ProveedorListComponent, RouterModule, ArsCurrencyPipe, PriceNoteComponent],
  templateUrl: './ver-proveedores.html',
  styleUrls: ['./ver-proveedores.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VerProveedoresComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly state = inject(VerProveedoresState);

  private readonly pedidoModalCard = viewChild<ElementRef<HTMLElement>>('pedidoModalCard');

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  termino = this.state.termino;
  filtroEstado = this.state.filtroEstado;
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
  pedidosListosParaRecibir = this.state.pedidosListosParaRecibir;
  loading = this.state.loading;
  loadingHistorial = this.state.loadingHistorial;
  loadingInsumos = this.state.loadingInsumos;
  error = this.state.error;
  errorHistorial = this.state.errorHistorial;
  errorInsumos = this.state.errorInsumos;
  historialProveedor = this.state.historialProveedor;

  faCheck = faCheck;
  faXmark = faXmark;
  readonly Math = Math;

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

  irARealizarPedidoSugeridoGeneral(): void {
    this.router.navigate(['/staff', 'gerente', 'realizar-pedido-sugerido']);
  }

  setFiltroEstado(estado: 'Todos' | 'Activos' | 'Inactivos'): void {
    this.filtroEstado.set(estado);
  }

  irARealizarPedidoSugerido(proveedor: Proveedor): void {
    this.router.navigate(['/staff', 'gerente', 'realizar-pedido-sugerido', proveedor.id]);
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

  mostrarHistorial(proveedor?: Proveedor): void {
    if (proveedor) {
      this.state.abrirHistorial(proveedor.id);
      return;
    }
    this.state.panelModo.set('historial');
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

  reintentarCarga(): void {
    this.state.cargarDatos();
  }

  reintentarProveedor(): void {
    const proveedorId = this.proveedorSeleccionadoId();
    if (proveedorId === null) return;
    this.state.cargarHistorial(proveedorId);
    this.state.cargarInsumosProveedor(proveedorId);
  }

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':
        return 'estado-recibido';
      case 'Enviado':
        return 'estado-enviado';
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

  cantidadDisplay(): number | null {
    const cantidad = this.cantidadProducto();
    if (cantidad === null) return null;
    return this.usaUnidadMenor() ? Math.round(cantidad * 1000) : cantidad;
  }

  setCantidadDisplay(valor: number | string | null): void {
    if (valor === null || valor === '') {
      this.cantidadProducto.set(null);
      return;
    }

    const cantidad = Number(valor);
    if (!Number.isFinite(cantidad)) {
      this.cantidadProducto.set(null);
      return;
    }

    this.cantidadProducto.set(this.usaUnidadMenor() ? cantidad / 1000 : cantidad);
  }

  ajustarCantidadDisplay(delta: number): void {
    const actual = this.cantidadDisplay() ?? 0;
    const siguiente = Math.max(actual + delta, this.cantidadMinimaDisplay());
    this.setCantidadDisplay(siguiente);
  }

  sumarCantidadPreset(valor: number): void {
    const actual = this.cantidadDisplay() ?? 0;
    this.setCantidadDisplay(actual + valor);
  }

  limpiarCantidadDisplay(): void {
    this.setCantidadDisplay(0);
  }

  cantidadMinimaDisplay(): number {
    return 0;
  }

  cantidadPasoDisplay(): number {
    if (this.esUnidadPeso()) return 100;
    if (this.esUnidadGramos()) return 10;
    if (this.esUnidadVolumen() || this.esUnidadMililitros()) return 100;
    return this.cantidadPasoProducto;
  }

  cantidadPlaceholderDisplay(): string {
    if (this.esUnidadPeso()) return 'Ej: 100';
    if (this.esUnidadGramos()) return 'Ej: 10';
    if (this.esUnidadVolumen() || this.esUnidadMililitros()) return 'Ej: 500';
    return this.cantidadPlaceholderProducto;
  }

  unidadDisplay(): string {
    if (this.esUnidadPeso()) return 'g';
    if (this.esUnidadGramos()) return 'gr';
    if (this.esUnidadVolumen() || this.esUnidadMililitros()) return 'ml';
    return this.nombreUnidad(this.productoBaseActual()?.unidadMedida).toLowerCase() || 'un';
  }

  unidadDisplayCompleta(): string {
    if (this.esUnidadPeso() || this.esUnidadGramos()) return 'gramos';
    if (this.esUnidadVolumen() || this.esUnidadMililitros()) return 'mililitros';

    const unidad = this.nombreUnidad(this.productoBaseActual()?.unidadMedida).trim().toUpperCase();
    if (['UN', 'UNIDAD', 'UNIDADES'].includes(unidad)) return 'unidades';
    if (['PORCION', 'PORCIONES'].includes(unidad)) return 'porciones';
    return this.nombreUnidad(this.productoBaseActual()?.unidadMedida).toLowerCase() || 'unidades';
  }

  presetsCantidad(): QuantityPreset[] {
    const producto = this.productoBaseActual();
    const fallback = this.presetsCantidadBase();
    if (!producto) return fallback;

    return buildSmartQuantityPresets(
      this.historialProveedor(),
      producto.id,
      this.nombreUnidad(producto.unidadMedida),
      fallback
    );
  }

  private presetsCantidadBase(): QuantityPreset[] {
    if (this.esUnidadPeso()) {
      return [
        { label: '100 g', value: 100 },
        { label: '500 g', value: 500 },
        { label: '1 kg', value: 1000 },
        { label: '10 kg', value: 10000 },
        { label: '50 kg', value: 50000 }
      ];
    }

    if (this.esUnidadGramos()) {
      return [
        { label: '100 gr', value: 100 },
        { label: '500 gr', value: 500 },
        { label: '1 kg', value: 1000 },
        { label: '10 kg', value: 10000 },
        { label: '50 kg', value: 50000 }
      ];
    }

    if (this.esUnidadVolumen() || this.esUnidadMililitros()) {
      return [
        { label: '100 ml', value: 100 },
        { label: '500 ml', value: 500 },
        { label: '1 l', value: 1000 },
        { label: '10 l', value: 10000 },
        { label: '50 l', value: 50000 }
      ];
    }

    return [];
  }

  nombreUnidad(unidadMedida: UnidadMedida | string | null | undefined): string {
    if (!unidadMedida) return '';
    return typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
  }

  nombreCategoria(categoria: CategoriaInsumo | string | null | undefined): string {
    if (!categoria) return '';
    return typeof categoria === 'string' ? categoria : categoria.descripcion;
  }

  equivalenciaCantidad(): string | null {
    const cantidad = this.cantidadProducto();
    const producto = this.productoBaseActual();
    if (!cantidad || cantidad <= 0 || !producto?.unidadMedida) return null;

    const unidad = this.nombreUnidad(producto.unidadMedida).trim().toUpperCase();
    if (['KG', 'KILO', 'KILOS'].includes(unidad)) {
      return `Se agregará como ${this.formatearEquivalencia(cantidad, 'kg', 'g', 1000)}`;
    }

    if (['L', 'LT', 'LITRO', 'LITROS'].includes(unidad)) {
      return `Se agregará como ${this.formatearEquivalencia(cantidad, 'l', 'ml', 1000)}`;
    }

    if (['ML', 'MILILITRO', 'MILILITROS'].includes(unidad) && cantidad >= 1000) {
      return `Equivale a ${this.formatearEquivalencia(cantidad / 1000, 'l', 'ml', 1000)}`;
    }

    return null;
  }

  private usaUnidadMenor(): boolean {
    return this.esUnidadPeso() || this.esUnidadVolumen();
  }

  private esUnidadPeso(): boolean {
    const unidad = this.nombreUnidad(this.productoBaseActual()?.unidadMedida).trim().toUpperCase();
    return ['KG', 'KILO', 'KILOS'].includes(unidad);
  }

  private esUnidadGramos(): boolean {
    const unidad = this.nombreUnidad(this.productoBaseActual()?.unidadMedida).trim().toUpperCase();
    return ['G', 'GR', 'GRAMO', 'GRAMOS'].includes(unidad);
  }

  private esUnidadVolumen(): boolean {
    const unidad = this.nombreUnidad(this.productoBaseActual()?.unidadMedida).trim().toUpperCase();
    return ['L', 'LT', 'LITRO', 'LITROS'].includes(unidad);
  }

  private esUnidadMililitros(): boolean {
    const unidad = this.nombreUnidad(this.productoBaseActual()?.unidadMedida).trim().toUpperCase();
    return ['ML', 'MILILITRO', 'MILILITROS'].includes(unidad);
  }

  private formatearEquivalencia(cantidad: number, unidadMayor: string, unidadMenor: string, factor: number): string {
    const enteros = Math.trunc(cantidad);
    const menores = Math.round((cantidad - enteros) * factor);

    if (enteros > 0 && menores > 0) return `${enteros} ${unidadMayor} ${menores} ${unidadMenor}`;
    if (enteros > 0) return `${enteros} ${unidadMayor}`;
    return `${Math.round(cantidad * factor)} ${unidadMenor}`;
  }
}
