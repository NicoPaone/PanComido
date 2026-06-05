import { Component, OnInit, inject , ChangeDetectionStrategy} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/domain/proveedor';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { RealizarPedidoSugeridoStateService } from '../services/realizar-pedido-sugerido.state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-realizar-pedido-sugerido',
  standalone: true,
  imports: [DecimalPipe, FormsModule, Boton, Buscador],
  templateUrl: './realizar-pedido-sugerido.html',
  styleUrls: ['./realizar-pedido-sugerido.css']
})
export class RealizarPedidoSugeridoComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly state = inject(RealizarPedidoSugeridoStateService);

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  proveedorId = this.state.proveedorId;
  proveedor = this.state.proveedor;
  proveedores = this.state.proveedores;
  sugerencias = this.state.sugerencias;
  pedidoItems = this.state.pedidoItems;
  observaciones = this.state.observaciones;
  busqueda = this.state.busqueda;
  busquedaProveedor = this.state.busquedaProveedor;
  mensajeError = this.state.mensajeError;

  montoEstimado = this.state.montoEstimado;
proveedoresFiltrados = this.state.proveedoresFiltrados;

  // TODO REFACTOR: panel "Agregar ingredientes" duplicado de historial-proveedor (03/06/2026)
  proveedorAgregarIngredienteId = this.state.proveedorAgregarIngredienteId;
  busquedaIngrediente = this.state.busquedaIngrediente;
  productoExtraSeleccionadoId = this.state.productoExtraSeleccionadoId;
  cantidadIngrediente = this.state.cantidadIngrediente;
  precioIngrediente = this.state.precioIngrediente;
  ingredientesParaAgregar = this.state.ingredientesParaAgregar;

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.state.proveedorId.set(+idParam);
    }
  }

  ngOnInit(): void {
    const id = this.state.proveedorId();
    this.state.cargarDatos(id > 0 ? id : undefined);
  }

  seleccionarProveedor(id: string | number): void {
    this.state.seleccionarProveedor(id);
  }

  onSearchChanged(query: string): void {
    this.state.setSearchTerm(query);
  }

  itemsProveedor(proveedorId: string | number): SugerenciaPedidoItem[] {
    return this.state.obtenerItemsProveedor(proveedorId);
  }

  montoProveedor(proveedorId: string | number): number {
    return this.state.calcularMontoProveedor(proveedorId);
  }

  observacionProveedor(proveedorId: string | number): string {
    return this.state.obtenerObservacionProveedor(proveedorId);
  }

  setObservacionProveedor(proveedorId: string | number, observacion: string): void {
    this.state.setObservacionProveedor(proveedorId, observacion);
  }

  eliminarItem(proveedorId: string | number, productoId: string): void {
    this.state.eliminarItem(proveedorId, productoId);
  }

  onCantidadCambiada(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    this.state.onCantidadCambiada(proveedorId, item, val);
  }

  onPrecioCambiado(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    this.state.onPrecioCambiado(proveedorId, item, val);
  }

  nombreUnidad(unidadMedida: UnidadMedida | string | null | undefined): string {
    if (!unidadMedida) return '';
    return typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
  }

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  enviarPedido(proveedor: Proveedor): void {
    this.state.enviarPedido(proveedor, () => {
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores', proveedor.id, 'historial'], {
        state: { created: true, message: 'Pedido creado correctamente. Quedó pendiente en el historial.' }
      });
    });
  }

  // TODO REFACTOR: panel "Agregar ingredientes" duplicado de historial-proveedor (03/06/2026)
  abrirAgregarIngrediente(proveedorId: number | string): void {
    this.state.abrirAgregarIngrediente(proveedorId);
  }

  cerrarAgregarIngrediente(): void {
    this.state.cerrarAgregarIngrediente();
  }

  seleccionarIngredienteExtra(productoId: string): void {
    this.state.seleccionarIngredienteExtra(productoId);
  }

  onCantidadIngredienteChange(val: number | null): void {
    this.state.setCantidadIngrediente(val);
  }

  onPrecioIngredienteChange(val: number | null): void {
    this.state.setPrecioIngrediente(val);
  }

  confirmarAgregarIngrediente(): void {
    this.state.confirmarAgregarIngrediente();
  }
}
