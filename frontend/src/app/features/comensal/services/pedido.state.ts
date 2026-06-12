import { Injectable, signal, computed } from '@angular/core';
import { ItemPedido } from '../../../core/models/domain/item-pedido';

@Injectable({
  providedIn: 'root'
})
export class PedidoState {

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
    // Agregar al signal (Angular detectará el cambio automáticamente)
    this.pedidosSignal.update(pedidos => [...pedidos, item]);
  }

  obtenerPedidos(): ItemPedido[] {
    return this.pedidosSignal();
  }

  actualizarObservaciones(index: number, observacionesIngredientes: string, observacionesGenerales: string) {
    this.pedidosSignal.update(pedidos => {
      const nuevoPedidos = [...pedidos];
      nuevoPedidos[index] = {
        ...nuevoPedidos[index],
        observacionesIngredientes,
        observacionesGenerales
      };
      return nuevoPedidos;
    });
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

  incrementarCantidad(index: number): void {
  this.pedidosSignal.update(pedidos => {
    const nuevosPedidos = [...pedidos];

    nuevosPedidos[index] = {
      ...nuevosPedidos[index],
      cantidad: nuevosPedidos[index].cantidad + 1
    };

    return nuevosPedidos;
  });
}

decrementarCantidad(index: number): void {
  this.pedidosSignal.update(pedidos => {
    const nuevosPedidos = [...pedidos];

    if (nuevosPedidos[index].cantidad > 1) {
      nuevosPedidos[index] = {
        ...nuevosPedidos[index],
        cantidad: nuevosPedidos[index].cantidad - 1
      };
    } else {
      nuevosPedidos.splice(index, 1);
    }

    return nuevosPedidos;
  });
}
}
