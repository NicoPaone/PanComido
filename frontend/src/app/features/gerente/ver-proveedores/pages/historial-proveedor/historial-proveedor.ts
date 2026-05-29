import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { PageToolbar } from '../../../../../shared/ui/page-toolbar/page-toolbar';
import { PedidoProveedor, EstadoPedidoProveedor } from '../../../../../core/models/proveedor';
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

  // Señales del state
  proveedorSeleccionado = this.state.proveedorSeleccionado;
  historialProveedor = this.state.historialProveedor;
  loadingHistorial = this.state.loadingHistorial;
  pedidoHistorialSeleccionado = this.state.pedidoHistorialSeleccionado;

  faXmark = faXmark;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.volver();
      return;
    }

    const proveedorId = parseInt(id, 10);

    // Si los proveedores aún no están cargados, cargarlos primero
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

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':  return 'estado-recibido';
      case 'Confirmado': return 'estado-confirmado';
      case 'Cancelado':  return 'estado-cancelado';
      default:           return 'estado-pendiente';
    }
  }
}
