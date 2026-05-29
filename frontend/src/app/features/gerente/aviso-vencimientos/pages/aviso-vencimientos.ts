import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { VencimientosStateService } from '../services/vencimientos.state';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { IngredienteVencimiento } from '../../../../core/models/vencimientos.model';

@Component({
  selector: 'app-vencimientos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton],
  templateUrl: './aviso-vencimientos.html',
  styleUrls: ['./aviso-vencimientos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VencimientosPage implements OnInit {
  state = inject(VencimientosStateService);
  router = inject(Router);
  isOffcanvasOpen = false;

  cantidadAgregar = 1;
  pedidoSeleccionadoId = '';

  ngOnInit() {
    this.state.cargarIngredientes();
  }

  abrirOffcanvas(ingrediente: IngredienteVencimiento) {
    this.cantidadAgregar = 1;
    this.pedidoSeleccionadoId = '';
    this.state.seleccionarIngredienteParaPedido(ingrediente);
    this.isOffcanvasOpen = true;
  }

  cerrarOffcanvas() {
    this.isOffcanvasOpen = false;
    this.state.limpiarSeleccion();
  }

  seleccionarProveedor(e: Event) {
    const pId = (e.target as HTMLSelectElement).value;
    const proveedores = this.state.proveedoresDisponibles();
    const proveedor = proveedores.find(p => p.id === pId);
    if (proveedor) {
      this.state.seleccionarProveedor(proveedor);
    }
  }

  setPedidoSeleccionado(e: Event) {
    this.pedidoSeleccionadoId = (e.target as HTMLSelectElement).value;
  }

  updateCantidad(e: Event) {
    this.cantidadAgregar = Number((e.target as HTMLInputElement).value);
  }

  confirmarAgregarPedido() {
    if (!this.pedidoSeleccionadoId) {
      alert("Seleccione un pedido activo");
      return;
    }
    this.state.agregarAlPedido(this.pedidoSeleccionadoId, this.cantidadAgregar);
    this.cerrarOffcanvas();
  }

  nuevoPedidoRedireccion() {
    const ing = this.state.ingredienteSeleccionado();
    if(ing) {
      this.router.navigate(['/gerente/crear-pedido'], {
        queryParams: { ingredienteId: ing.id, cantidad: this.cantidadAgregar }
      });
      this.cerrarOffcanvas();
    }
  }
}