import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/insumos/insumo';
import { PedidoSugeridoIAStateService } from '../services/pedido-sugerido-ia.state';

@Component({
  selector: 'app-pedido-sugerido-ia',
  standalone: true,
  imports: [DecimalPipe, FormsModule, Boton, Buscador],
  templateUrl: './pedido-sugerido-ia.html',
  styleUrls: ['./pedido-sugerido-ia.css']
})
export class PedidoSugeridoIAComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly state = inject(PedidoSugeridoIAStateService);

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  proveedorId = this.state.proveedorId;
  proveedor = this.state.proveedor;
  sugerencias = this.state.sugerencias;
  pedidoItems = this.state.pedidoItems;
  observaciones = this.state.observaciones;
  busqueda = this.state.busqueda;
  productosDisponibles = this.state.productosDisponibles;

  montoEstimado = this.state.montoEstimado;
  sugerenciasExtras = this.state.sugerenciasExtras;

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.state.proveedorId.set(+idParam);
    }
  }

  ngOnInit(): void {
    const id = this.state.proveedorId();
    this.state.cargarDatos(id, () => this.volver());
  }

  onSearchChanged(query: string): void {
    this.state.setSearchTerm(query);
  }

  agregarProductoManual(prod: ProductoStockMock): void {
    this.state.agregarProductoManual(prod);
  }

  eliminarItem(productoId: string): void {
    this.state.eliminarItem(productoId);
  }

  onCantidadCambiada(item: SugerenciaPedidoItem, val: number | null): void {
    this.state.onCantidadCambiada(item, val);
  }

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  enviarPedido(): void {
    this.state.enviarPedido(() => {
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], {
        state: { created: true, message: 'Pedido sugerido por IA enviado correctamente' }
      });
    });
  }
}
