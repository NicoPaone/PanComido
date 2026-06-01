import { Component, ChangeDetectionStrategy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Aviso } from '../../../../core/models/aviso.model';
import { UnidadMedida } from '../../../../core/models/producto-stock';
import { VencimientosStateService } from '../../aviso-vencimientos/services/vencimientos.state';
import { AvisosStateService } from '../services/avisos.state';

@Component({
  selector: 'app-avisos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton, Buscador],
  templateUrl: './avisos.html',
  styleUrls: ['./avisos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AvisosPage implements OnInit {
  state = inject(AvisosStateService);
  pedidoState = inject(VencimientosStateService);
  router = inject(Router);
  isPedidoOffcanvasOpen = false;
  cantidadAgregar = 1;
  stockAvisoSeleccionado: Aviso | null = null;

  ngOnInit(): void {
    this.state.cargarSugerenciasCocina();
  }

  onBuscar(term: string) {
    this.state.setSearchTerm(term);
  }

  abrirAviso(a: Aviso) {
    this.abrirVencimientos(a.id);
  }

  abrirVencimientos(avisoId: string) {
    this.router.navigate(['/staff/gerente/aviso-vencimientos'], { queryParams: { avisoId } });
  }

  abrirPedidoStock(aviso: Aviso) {
    this.stockAvisoSeleccionado = aviso;
    this.cantidadAgregar = this.getCantidadInicial(aviso);
    this.pedidoState.seleccionarIngredienteParaPedido({
      id: aviso.id,
      nombre: aviso.titulo,
      fechaVencimiento: '',
      stockDisponible: this.getStockDisponible(aviso),
      unidadMedida: this.getUnidadMedida(aviso)
    });
    this.isPedidoOffcanvasOpen = true;
  }

  cerrarPedidoStock() {
    this.isPedidoOffcanvasOpen = false;
    this.stockAvisoSeleccionado = null;
    this.pedidoState.limpiarSeleccion();
  }

  seleccionarProveedor(e: Event) {
    const proveedorId = (e.target as HTMLSelectElement).value;
    const proveedor = this.pedidoState.proveedoresDisponibles().find(item => item.id.toString() === proveedorId);
    if (proveedor) {
      this.pedidoState.seleccionarProveedor(proveedor);
    }
  }

  updateCantidad(e: Event) {
    this.cantidadAgregar = Number((e.target as HTMLInputElement).value);
  }

  confirmarPedidoStock() {
    const pedido = this.pedidoState.crearPedidoPendiente(this.cantidadAgregar);
    const aviso = this.stockAvisoSeleccionado;
    if (!pedido || !aviso) return;

    pedido.pipe(take(1)).subscribe(proveedor => {
      this.state.marcarRevisado(aviso.tipo, aviso.id);
      this.cerrarPedidoStock();
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores', proveedor.id, 'historial']);
    });
  }

  cerrarConfirmacionCarta(): void {
    this.state.cerrarConfirmacionCarta();
  }

  irAModificarCarta(): void {
    this.state.cerrarConfirmacionCarta();
    this.router.navigate(['/staff', 'gerente', 'modificar-carta']);
  }

  private getStockDisponible(aviso: Aviso): number {
    const match = aviso.subtitulo?.match(/[\d.]+/);
    return match ? Number(match[0]) : 1;
  }

  private getUnidadMedida(aviso: Aviso): UnidadMedida {
    if (aviso.subtitulo?.includes(' L')) return 'L';
    if (aviso.subtitulo?.includes(' UN')) return 'UN';
    if (aviso.subtitulo?.includes(' GR')) return 'GR';
    return 'KG';
  }

  private getCantidadInicial(aviso: Aviso): number {
    return this.getUnidadMedida(aviso) === 'UN' ? 1 : 1;
  }
}
