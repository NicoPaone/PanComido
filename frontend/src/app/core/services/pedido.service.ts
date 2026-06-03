import { Injectable, signal, computed } from '@angular/core';
import { ItemPedido } from '../models/item-pedido';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {

  // Signal reactivo para los pedidos
  private pedidosSignal = signal<ItemPedido[]>([]);

  // Exponer como readonly
  pedidos = this.pedidosSignal.asReadonly();

  // Computed para cantidad total
  cantidadTotal = computed(() => {
    return this.pedidosSignal().reduce((acc, item) => acc + item.cantidad, 0);
  });

  // Computed para precio total
  totalPrecio = computed(() => {
    return this.pedidosSignal().reduce(
      (acc, item) => acc + (item.plato.precioVentaFinal * item.cantidad),
      0
    );
  });

  agregarPedido(item: ItemPedido) {
    console.log('Agregando:', item.plato.nombre, 'Cantidad:', item.cantidad);
    
    // Agregar al signal (Angular detectará el cambio automáticamente)
    this.pedidosSignal.update(pedidos => [...pedidos, item]);
    
    console.log('Pedidos actuales:', this.pedidosSignal().length);
    console.log('Total items:', this.cantidadTotal());
    console.log('Total precio:', this.totalPrecio());
  }

  obtenerPedidos(): ItemPedido[] {
    return this.pedidosSignal();
  }

  eliminarPedido(index: number) {
    this.pedidosSignal.update(pedidos => {
      const nuevoPedidos = [...pedidos];
      nuevoPedidos.splice(index, 1);
      return nuevoPedidos;
    });
  }

  limpiarPedidos() {
    this.pedidosSignal.set([]);
  }
}
