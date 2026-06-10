import { DestroyRef, Injectable, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { VencimientosApiService } from './vencimientos.api';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/domain/vencimiento';
import { PedidoProveedorRequest } from '../../../../core/models/domain/proveedor';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VencimientosState {
  private api = inject(VencimientosApiService);
  private destroyRef = inject(DestroyRef);
  private readonly preciosIngredientes: Record<string, number> = {
    '1': 1200,
    '2': 900,
    '3': 1500,
    '4': 600,
    '5': 1100,
    '6': 7500,
    '7': 120,
    '8': 300,
    '9': 800,
    '10': 700,
    '11': 4500
  };

  readonly #ingredientes = signal<IngredienteVencimiento[]>([]);
  readonly #loadingIngredientes = signal<boolean>(false);
  readonly #mensaje = signal<string | null>(null);

  readonly #proveedoresDisponibles = signal<VencimientoProveedor[]>([]);
  readonly #pedidosActivos = signal<VencimientoPedidoActivo[]>([]);
  
  readonly #ingredienteSeleccionado = signal<IngredienteVencimiento | null>(null);
  readonly #proveedorSeleccionado = signal<VencimientoProveedor | null>(null);

  ingredientes = this.#ingredientes.asReadonly();
  loadingIngredientes = this.#loadingIngredientes.asReadonly();
  mensaje = this.#mensaje.asReadonly();
  proveedoresDisponibles = this.#proveedoresDisponibles.asReadonly();
  pedidosActivos = this.#pedidosActivos.asReadonly();
  ingredienteSeleccionado = this.#ingredienteSeleccionado.asReadonly();
  proveedorSeleccionado = this.#proveedorSeleccionado.asReadonly();

  cargarIngredientes() {
    this.#loadingIngredientes.set(true);
    this.api.getIngredientesProximosVencer().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(data => {
      this.#ingredientes.set(data);
      this.#loadingIngredientes.set(false);
    });
  }

  seleccionarIngredienteParaPedido(ingrediente: IngredienteVencimiento) {
    this.#ingredienteSeleccionado.set(ingrediente);
    this.#proveedorSeleccionado.set(null);
    this.#pedidosActivos.set([]);
    this.api.getProveedoresPorIngrediente(ingrediente.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe(provs => {
      this.#proveedoresDisponibles.set(provs);
    });
  }

  seleccionarProveedor(proveedor: VencimientoProveedor) {
    this.#proveedorSeleccionado.set(proveedor);
    this.api.getPedidosActivosPorProveedor(proveedor.id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe(pedidos => {
      this.#pedidosActivos.set(pedidos);
    });
  }

  limpiarSeleccion() {
    this.#ingredienteSeleccionado.set(null);
    this.#proveedorSeleccionado.set(null);
    this.#proveedoresDisponibles.set([]);
    this.#pedidosActivos.set([]);
  }

  marcarRevisado(ingredienteId: string | number) {
    this.#ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Vencimiento revisado');
  }

  sugerirUso(ingredienteId: string | number) {
    this.#ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Sugerencia de uso enviada');
  }

  avisarCocina(ingredienteId: string | number) {
    this.#ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Cocina notificada');
  }

  crearPedidoPendiente(cantidad: number): Observable<VencimientoProveedor> | null {
    const ingrediente = this.#ingredienteSeleccionado();
    const proveedor = this.#proveedorSeleccionado();
    if (!ingrediente || !proveedor) return null;

    const precioUnitario = this.preciosIngredientes[ingrediente.id.toString()] ?? 500;

    const pedido: PedidoProveedorRequest = {
      proveedorId: proveedor.id,
      concepto: `Pedido por vencimiento: ${ingrediente.nombre}`,
      monto: precioUnitario * cantidad,
      observacion: 'Pedido inicial generado desde vencimientos',
      items: [{
        id: ingrediente.id,
        nombre: ingrediente.nombre,
        cantidad,
        unidadMedida: ingrediente.unidadMedida,
        precioUnitario
      }]
    };

    return this.api.crearPedidoProveedor(proveedor.id, pedido).pipe(map(() => proveedor));
  }

  private mostrarMensaje(mensaje: string) {
    this.#mensaje.set(mensaje);
    setTimeout(() => this.#mensaje.set(null), 2500);
  }
}
