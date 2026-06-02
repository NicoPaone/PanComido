import { Component, OnInit, inject, ChangeDetectionStrategy, computed, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { FormsModule } from '@angular/forms';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { PageToolbar } from '../../../../../shared/ui/page-toolbar/page-toolbar';
import { PedidoProveedor, EstadoPedidoProveedor } from '../../../../../core/models/proveedor';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { UnidadMedida } from '../../../../../core/models/unidad-medida';
import { VerProveedoresStateService } from '../../services/ver-proveedores.state';

interface IngredientePickerItem {
  id: string;
  producto: Insumo;
  nombre: string;
  stock: string;
  vencimiento: string | null;
}

@Component({
  selector: 'app-historial-proveedor',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, FontAwesomeModule, Boton, PageToolbar],
  templateUrl: './historial-proveedor.html',
  styleUrls: ['./historial-proveedor.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HistorialProveedorComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly state = inject(VerProveedoresStateService);

  proveedorSeleccionado = this.state.proveedorSeleccionado;
  historialProveedor = this.state.historialProveedor;
  loadingHistorial = this.state.loadingHistorial;
  errorHistorial = this.state.errorHistorial;
  pedidoHistorialSeleccionado = this.state.pedidoHistorialSeleccionado;
  mensajeAccion = this.state.mensajeAccion;
  recepcionPedido = this.state.recepcionPedido;
  recepcionItems = this.state.recepcionItems;
  bodegas = this.state.bodegas;

  faXmark = faXmark;
  isAgregarIngredientesOpen = signal(false);
  busquedaIngrediente = signal('');
  productoSeleccionadoId = signal('');
  cantidadIngrediente = signal(1);

  ingredientesParaAgregar = computed<IngredientePickerItem[]>(() => {
    const texto = this.busquedaIngrediente().toLowerCase().trim();
    const pedido = this.pedidoHistorialSeleccionado();
    const vistos = new Set<string>();

    return this.state.productos()
      .reduce<IngredientePickerItem[]>((items, producto) => {
        const nombre = producto.nombre?.trim() ?? '';
        const id = producto.id?.toString();
        if (!nombre || !id || vistos.has(id)) return items;
        vistos.add(id);

        if (pedido?.items.some(item => item.id.toString() === id)) return items;
        if (texto && !nombre.toLowerCase().includes(texto)) return items;

        items.push({
          id,
          producto,
          nombre,
          stock: `${producto.stockActual} ${this.nombreUnidad(producto.unidadMedida)} disponibles`,
          vencimiento: producto.vencimiento?.trim() || null
        });
        return items;
      }, [])
      .sort((a, b) => a.nombre.localeCompare(b.nombre));
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.volver();
      return;
    }

    const proveedorId = parseInt(id, 10);

    // Cargar lista de proveedores si aún no está disponible (solo metadatos, sin efectos sobre insumos)
    if (this.state.proveedores().length === 0) {
      this.state.cargarProveedoresSolos();
    }

    // Siempre seleccionar el proveedor correcto y cargar SUS insumos e historial
    this.state.seleccionarProveedor(proveedorId);
    this.state.cargarHistorial(proveedorId);
  }

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  reintentarHistorial(): void {
    const proveedor = this.proveedorSeleccionado();
    if (!proveedor) return;
    this.state.cargarHistorial(proveedor.id);
  }

  abrirDetallePedido(pedido: PedidoProveedor): void {
    this.state.abrirDetallePedido(pedido);
  }

  cerrarDetallePedido(): void {
    this.state.cerrarDetallePedido();
  }

  confirmarPedido(event: MouseEvent, pedido: PedidoProveedor): void {
    event.stopPropagation();
    this.state.confirmarPedido(pedido);
  }

  previsualizarRecepcion(event: MouseEvent, pedido: PedidoProveedor): void {
    event.stopPropagation();
    this.state.previsualizarRecepcion(pedido);
  }

  cerrarRecepcion(): void {
    this.state.cerrarRecepcion();
  }

  actualizarCantidadRecepcion(insumoId: number, value: string | number | null): void {
    const cantidad = Number(value);
    if (Number.isFinite(cantidad)) {
      this.state.actualizarRecepcionItem(insumoId, { cantidad });
    }
  }

  actualizarFechaRecepcion(insumoId: number, fechaVencimiento: string): void {
    this.state.actualizarRecepcionItem(insumoId, { fechaVencimiento });
  }

  actualizarBodegaRecepcion(insumoId: number, value: string | number): void {
    this.state.actualizarRecepcionItem(insumoId, { bodegaId: Number(value) });
  }

  recibirPedido(): void {
    this.state.recibirPedido();
  }

  agregarIngredientes(): void {
    this.isAgregarIngredientesOpen.set(true);
    this.busquedaIngrediente.set('');
    this.productoSeleccionadoId.set('');
    this.cantidadIngrediente.set(1);
  }

  cerrarAgregarIngredientes(): void {
    this.isAgregarIngredientesOpen.set(false);
  }

  onBusquedaIngrediente(event: Event): void {
    this.busquedaIngrediente.set((event.target as HTMLInputElement).value);
  }

  seleccionarIngrediente(producto: Insumo): void {
    this.productoSeleccionadoId.set(producto.id.toString());
    this.busquedaIngrediente.set(producto.nombre);
    this.cantidadIngrediente.set(this.getCantidadConfiguracion(producto.unidadMedida).min);
  }

  updateCantidadIngrediente(event: Event): void {
    const input = event.target as HTMLInputElement;
    const cantidad = Number(input.value);
    if (!Number.isFinite(cantidad)) return;

    const producto = this.productoSeleccionado();
    const unidad = this.nombreUnidad(producto?.unidadMedida).trim().toUpperCase();
    const cantidadNormalizada = ['UN', 'UNIDAD', 'UNIDADES', 'PORCION', 'PORCIONES'].includes(unidad)
      ? Math.max(1, Math.round(cantidad))
      : cantidad;

    this.cantidadIngrediente.set(cantidadNormalizada);
    input.value = cantidadNormalizada.toString();
  }

  nombreUnidad(unidadMedida: UnidadMedida | string | null | undefined): string {
    if (!unidadMedida) return '';
    return typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
  }

  productoSeleccionado(): Insumo | null {
    const productoId = this.productoSeleccionadoId();
    if (!productoId) return null;
    return this.state.productos().find(producto => producto.id.toString() === productoId) ?? null;
  }

  get cantidadIngredientePaso(): number {
    return this.getCantidadConfiguracion(this.productoSeleccionado()?.unidadMedida ?? 'Kilos').step;
  }

  get cantidadIngredienteMinima(): number {
    return this.getCantidadConfiguracion(this.productoSeleccionado()?.unidadMedida ?? 'Kilos').min;
  }

  get cantidadIngredientePlaceholder(): string {
    return this.getCantidadConfiguracion(this.productoSeleccionado()?.unidadMedida ?? 'Kilos').placeholder;
  }

  equivalenciaIngrediente(): string | null {
    const producto = this.productoSeleccionado();
    if (!producto) return null;
    return this.equivalenciaCantidad(this.cantidadIngrediente(), producto.unidadMedida);
  }

  cantidadConEquivalencia(cantidad: number, unidadMedida: UnidadMedida | string | null | undefined): string {
    const unidad = this.nombreUnidad(unidadMedida);
    const equivalencia = this.equivalenciaCantidad(cantidad, unidadMedida);
    return equivalencia ? `${cantidad} ${unidad} (${equivalencia.replace('Equivale a ', '')})` : `${cantidad} ${unidad}`;
  }

  cantidadItemPedido(cantidad: number, itemId: string | number, unidadMedida: UnidadMedida | string | null | undefined): string {
    const producto = this.state.productos().find(item => item.id.toString() === itemId.toString());
    return this.cantidadConEquivalencia(cantidad, producto?.unidadMedida ?? unidadMedida);
  }

  private getCantidadConfiguracion(unidadMedida: UnidadMedida | string): { step: number; min: number; placeholder: string } {
    const unidad = this.nombreUnidad(unidadMedida).trim().toUpperCase();
    if (['UN', 'UNIDAD', 'UNIDADES', 'PORCION', 'PORCIONES'].includes(unidad)) {
      return { step: 1, min: 1, placeholder: '1' };
    }

    if (['GR', 'GRAMO', 'GRAMOS'].includes(unidad)) {
      return { step: 10, min: 10, placeholder: '100' };
    }

    return { step: 0.1, min: 0.1, placeholder: '0.5' };
  }

  private equivalenciaCantidad(cantidad: number, unidadMedida: UnidadMedida | string | null | undefined): string | null {
    if (!cantidad || cantidad <= 0) return null;

    const unidad = this.nombreUnidad(unidadMedida).trim().toUpperCase();
    if (['KG', 'KILO', 'KILOS'].includes(unidad)) {
      return `Equivale a ${this.formatearEquivalencia(cantidad, 'kg', 'g', 1000)}`;
    }

    if (['L', 'LT', 'LITRO', 'LITROS'].includes(unidad)) {
      return `Equivale a ${this.formatearEquivalencia(cantidad, 'l', 'ml', 1000)}`;
    }

    return null;
  }

  private formatearEquivalencia(cantidad: number, unidadMayor: string, unidadMenor: string, factor: number): string {
    const enteros = Math.trunc(cantidad);
    const menores = Math.round((cantidad - enteros) * factor);

    if (enteros > 0 && menores > 0) return `${enteros} ${unidadMayor} ${menores} ${unidadMenor}`;
    if (enteros > 0) return `${enteros} ${unidadMayor}`;
    return `${Math.round(cantidad * factor)} ${unidadMenor}`;
  }

  confirmarAgregarIngrediente(pedido: PedidoProveedor): void {
    this.state.agregarIngredienteAPedido(pedido, this.productoSeleccionadoId(), this.cantidadIngrediente());
    this.cerrarAgregarIngredientes();
  }

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':  return 'estado-recibido';
      case 'Enviado': return 'estado-enviado';
      default:           return 'estado-pendiente';
    }
  }
}
