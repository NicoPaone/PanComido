import { Injectable, inject, signal } from '@angular/core';
import { VencimientosApiService } from './vencimientos.api';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/domain/vencimiento';
import { NuevoPedidoProveedor } from '../../../../core/models/dtos/requests/proveedor.request';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VencimientosState {
  private api = inject(VencimientosApiService);
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

  private _ingredientes = signal<IngredienteVencimiento[]>([]);
  private _loadingIngredientes = signal<boolean>(false);
  private _mensaje = signal<string | null>(null);

  private _proveedoresDisponibles = signal<VencimientoProveedor[]>([]);
  private _pedidosActivos = signal<VencimientoPedidoActivo[]>([]);
  
  private _ingredienteSeleccionado = signal<IngredienteVencimiento | null>(null);
  private _proveedorSeleccionado = signal<VencimientoProveedor | null>(null);

  ingredientes = this._ingredientes.asReadonly();
  loadingIngredientes = this._loadingIngredientes.asReadonly();
  mensaje = this._mensaje.asReadonly();
  proveedoresDisponibles = this._proveedoresDisponibles.asReadonly();
  pedidosActivos = this._pedidosActivos.asReadonly();
  ingredienteSeleccionado = this._ingredienteSeleccionado.asReadonly();
  proveedorSeleccionado = this._proveedorSeleccionado.asReadonly();

  cargarIngredientes() {
    this._loadingIngredientes.set(true);
    this.api.getIngredientesProximosVencer().subscribe(data => {
      this._ingredientes.set(data);
      this._loadingIngredientes.set(false);
    });
  }

  seleccionarIngredienteParaPedido(ingrediente: IngredienteVencimiento) {
    this._ingredienteSeleccionado.set(ingrediente);
    this._proveedorSeleccionado.set(null);
    this._pedidosActivos.set([]);
    this.api.getProveedoresPorIngrediente(ingrediente.id).subscribe(provs => {
      this._proveedoresDisponibles.set(provs);
    });
  }

  seleccionarProveedor(proveedor: VencimientoProveedor) {
    this._proveedorSeleccionado.set(proveedor);
    this.api.getPedidosActivosPorProveedor(proveedor.id).subscribe(pedidos => {
      this._pedidosActivos.set(pedidos);
    });
  }

  limpiarSeleccion() {
    this._ingredienteSeleccionado.set(null);
    this._proveedorSeleccionado.set(null);
    this._proveedoresDisponibles.set([]);
    this._pedidosActivos.set([]);
  }

  marcarRevisado(ingredienteId: string | number) {
    this._ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Vencimiento revisado');
  }

  sugerirUso(ingredienteId: string | number) {
    this._ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Sugerencia de uso enviada');
  }

  avisarCocina(ingredienteId: string | number) {
    this._ingredientes.update(items => items.filter(item => item.id !== ingredienteId));
    this.mostrarMensaje('Cocina notificada');
  }

  crearPedidoPendiente(cantidad: number): Observable<VencimientoProveedor> | null {
    const ingrediente = this._ingredienteSeleccionado();
    const proveedor = this._proveedorSeleccionado();
    if (!ingrediente || !proveedor) return null;

    const precioUnitario = this.preciosIngredientes[ingrediente.id.toString()] ?? 500;

    const pedido: NuevoPedidoProveedor = {
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
    this._mensaje.set(mensaje);
    setTimeout(() => this._mensaje.set(null), 2500);
  }
}
