import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';

interface PedidoItem {
  plato: {
    nombre: string;
    precioVenta: number;
  };
  cantidad: number;
}

@Component({
  selector: 'app-detalle-pedido',
  standalone: true,
  imports: [BotonComensal, LlamarAlMozo],
  templateUrl: './detalle-pedido.html',
  styleUrls: ['./detalle-pedido.css']
})
export class DetallePedido {
  private router = inject(Router);
  private pedidoService = inject(PedidoService);

  configuracion = configuracionRestauranteMock;
  pedidos = signal<PedidoItem[]>([]);

  constructor() {
    this.pedidos.set(this.pedidoService.obtenerPedidos());
  }

  get total(): number {
    return this.pedidos().reduce(
      (acc, item) => acc + item.plato.precioVenta * item.cantidad,
      0
    );
  }

  volver(): void {
    this.router.navigate(['/comensal/pedido']);
  }

  editar(item: PedidoItem): void {
    this.router.navigate(['/comensal/personalizar-plato']);
  }

  confirmarPedido(): void {
    alert('Pedido confirmado');
  }
}
