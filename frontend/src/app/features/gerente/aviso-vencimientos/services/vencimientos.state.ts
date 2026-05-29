import { Injectable, inject, signal } from '@angular/core';
import { VencimientosApiService } from './vencimientos.service';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/vencimientos.model';

@Injectable({ providedIn: 'root' })
export class VencimientosStateService {
  private api = inject(VencimientosApiService);

  // Signals
  private _ingredientes = signal<IngredienteVencimiento[]>([]);
  private _loadingIngredientes = signal<boolean>(false);

  private _proveedoresDisponibles = signal<VencimientoProveedor[]>([]);
  private _pedidosActivos = signal<VencimientoPedidoActivo[]>([]);
  
  private _ingredienteSeleccionado = signal<IngredienteVencimiento | null>(null);
  private _proveedorSeleccionado = signal<VencimientoProveedor | null>(null);

  // Readonly
  ingredientes = this._ingredientes.asReadonly();
  loadingIngredientes = this._loadingIngredientes.asReadonly();
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
    
    // Cargar proveedores que venden este ingrediente
    this.api.getProveedoresPorIngrediente(ingrediente.id).subscribe(provs => {
      this._proveedoresDisponibles.set(provs);
    });
  }

  seleccionarProveedor(proveedor: VencimientoProveedor) {
    this._proveedorSeleccionado.set(proveedor);
    // Cargar pedidos activos de este proveedor
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

  agregarAlPedido(pedidoId: string, cantidad: number) {
    const ingrediente = this._ingredienteSeleccionado();
    if (!ingrediente) return;

    this.api.agregarAPedidoExistente(pedidoId, ingrediente.id, cantidad).subscribe(() => {
      // Mock confirmación
      alert(`¡Agregado al pedido ${pedidoId} con éxito!`);
      this.limpiarSeleccion();
    });
  }
}
