import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { PageToolbar } from '../../../../../shared/ui/page-toolbar/page-toolbar';
import { PedidoProveedor, EstadoPedidoProveedor } from '../../../../../core/models/proveedor';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { VerProveedoresStateService } from '../../services/ver-proveedores.state';

@Component({
  selector: 'app-historial-proveedor',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FontAwesomeModule, Boton, PageToolbar],
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
  pedidoHistorialSeleccionado = this.state.pedidoHistorialSeleccionado;

  faXmark = faXmark;
  isAgregarIngredientesOpen = false;
  busquedaIngrediente = '';
  productoSeleccionadoId = '';
  cantidadIngrediente = 1;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.volver();
      return;
    }

    const proveedorId = parseInt(id, 10);

    if (this.state.proveedores().length === 0) {
      this.state.cargarDatos();
    }
    this.state.seleccionarProveedor(proveedorId);
    this.state.cargarHistorial(proveedorId);
  }

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
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

  agregarIngredientes(): void {
    this.isAgregarIngredientesOpen = true;
    this.busquedaIngrediente = '';
    this.productoSeleccionadoId = '';
    this.cantidadIngrediente = 1;
  }

  cerrarAgregarIngredientes(): void {
    this.isAgregarIngredientesOpen = false;
  }

  onBusquedaIngrediente(event: Event): void {
    this.busquedaIngrediente = (event.target as HTMLInputElement).value;
  }

  seleccionarIngrediente(producto: Insumo): void {
    this.productoSeleccionadoId = producto.id.toString();
    this.busquedaIngrediente = producto.nombre;
    this.cantidadIngrediente = producto.unidadMedida.nombre === 'UN' ? 1 : 0.5;
  }

  updateCantidadIngrediente(event: Event): void {
    this.cantidadIngrediente = Number((event.target as HTMLInputElement).value);
  }

  productosParaAgregar(): Insumo[] {
    return this.state.productosParaAgregar(this.busquedaIngrediente);
  }

  confirmarAgregarIngrediente(pedido: PedidoProveedor): void {
    this.state.agregarIngredienteAPedido(pedido, this.productoSeleccionadoId, this.cantidadIngrediente);
    this.cerrarAgregarIngredientes();
  }

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':  return 'estado-recibido';
      case 'Confirmado': return 'estado-confirmado';
      case 'Cancelado':  return 'estado-cancelado';
      default:           return 'estado-pendiente';
    }
  }
}
