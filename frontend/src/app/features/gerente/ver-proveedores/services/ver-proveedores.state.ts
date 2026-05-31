import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { VerProveedoresApiService } from './ver-proveedores.api';
import { Proveedor, PedidoProveedor, PedidoProveedorItem, EstadoPedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo } from '../../../../core/models/insumos/insumo';
import { UnidadMedida } from '../../../../core/models/unidad-medida';

@Injectable({ providedIn: 'root' })
export class VerProveedoresStateService {
  private api = inject(VerProveedoresApiService);
  private destroyRef = inject(DestroyRef);

  private readonly preciosMock: Record<string, number> = {
    '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100,
    '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
  };

  termino = signal('');
  proveedores = signal<Proveedor[]>([]);
  productos = signal<Insumo[]>([]);
  proveedorSeleccionadoId = signal<number | string | null>(null);
  panelModo = signal<'pedido' | 'historial'>('historial');
  observacionPedido = signal('');
  mensajeAccion = signal<string | null>(null);
  productoTexto = signal('');
  productoSeleccionadoId = signal<string |number|  null>(null);
  cantidadProducto = signal<number | null>(1);
  precioProductoManual = signal<number | null>(null);
  pedidoItems = signal<PedidoProveedorItem[]>([]);
  pedidoHistorialSeleccionado = signal<PedidoProveedor | null>(null);
  
  private _loading = signal(false);
  loading = this._loading.asReadonly();

  private _historialProveedor = signal<PedidoProveedor[]>([]);
  historialProveedor = this._historialProveedor.asReadonly();

  private _loadingHistorial = signal(false);
  loadingHistorial = this._loadingHistorial.asReadonly();

  proveedoresFiltrados = computed(() => {
    const texto = this.termino().toLowerCase().trim();
    const lista = [...this.proveedores()].sort((a, b) => {
      const fechaA = a.fechaUltimoPedido ? new Date(a.fechaUltimoPedido).getTime() : 0;
      const fechaB = b.fechaUltimoPedido ? new Date(b.fechaUltimoPedido).getTime() : 0;
      return fechaB - fechaA;
    });

    if (!texto) return lista;
    return lista.filter(proveedor => proveedor.nombre.toLowerCase().includes(texto));
  });

  productosFiltrados = computed(() => {
    const texto = this.productoTexto().toLowerCase().trim();
    const lista = [...this.productos()];

    if (!texto) return lista;
    return lista.filter(producto => producto.nombre.toLowerCase().includes(texto));
  });

  productoBaseActual = computed(() => {
    const productoSeleccionado = this.productos().find(producto => producto.id === this.productoSeleccionadoId());
    if (productoSeleccionado) return productoSeleccionado;

    const textoNormalizado = this.productoTexto().toLowerCase().trim();
    if (!textoNormalizado) return null;

    const coincidencias = this.productosFiltrados();
    const exacto = coincidencias.find(producto => producto.nombre.toLowerCase() === textoNormalizado);
    if (exacto) return exacto;

    return coincidencias.length === 1 ? coincidencias[0] : null;
  });

  proveedorSeleccionado = computed(() => {
    const proveedorId = this.proveedorSeleccionadoId();
    if (proveedorId === null) {
      return this.proveedoresFiltrados()[0] ?? null;
    }
    return this.proveedores().find(proveedor => proveedor.id === proveedorId) ?? null;
  });

  totalPedidosSeleccionado = computed(() => {
    return this._historialProveedor().length;
  });

  montoEstimado = computed(() => {
    return this.pedidoItems().reduce((total, item) => {
      const base = item.precioUnitario ?? this.preciosMock[item.id] ?? 500;
      return total + base * item.cantidad;
    }, 0);
  });

  productoSeleccionadoActual = computed(() => {
    return this.productos().find(producto => producto.id === this.productoSeleccionadoId()) ?? null;
  });

  cantidadPasoProducto = computed(() => {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').step;
  });

  cantidadMinimaProducto = computed(() => {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').min;
  });

  cantidadPlaceholderProducto = computed(() => {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').placeholder;
  });

  cargarDatos(): void {
    this._loading.set(true);
    this.api.getProveedores()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (provs) => {
          this.proveedores.set(provs);
          if (provs.length > 0 && this.proveedorSeleccionadoId() === null) {
            this.proveedorSeleccionadoId.set(provs[0].id);
          }
          this._loading.set(false);
        },
        error: () => this._loading.set(false)
      });

    this.api.getProductosDisponibles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(prods => {
        this.productos.set(prods);
      });
  }

  seleccionarProveedor(proveedorId: number | string): void {
    this.proveedorSeleccionadoId.set(proveedorId);
    this.mensajeAccion.set(null);
    this.pedidoHistorialSeleccionado.set(null);
    this._historialProveedor.set([]);
  }

  cargarHistorial(id: number | string): void {
    this._loadingHistorial.set(true);
    this.api.getHistorialPedidos(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (pedidos) => {
          this._historialProveedor.set(pedidos);
          this._loadingHistorial.set(false);
        },
        error: () => {
          this._historialProveedor.set([]);
          this._loadingHistorial.set(false);
        }
      });
  }

  abrirPedido(proveedorId: number | string): void {
    this.seleccionarProveedor(proveedorId);
    this.panelModo.set('pedido');
    this.mensajeAccion.set(null);
  }

  abrirHistorial(proveedorId: number | string): void {
    this.seleccionarProveedor(proveedorId);
    this.panelModo.set('historial');
  }

  abrirDetallePedido(pedido: PedidoProveedor): void {
    this.pedidoHistorialSeleccionado.set(pedido);
  }

  cerrarDetallePedido(): void {
    this.pedidoHistorialSeleccionado.set(null);
  }

  productosParaAgregar(busqueda: string): Insumo[] {
    const texto = busqueda.toLowerCase().trim();
    const productos = [...this.productos()].sort((a, b) => {
      const sugeridoA = this.esProductoSugerido(a) ? 0 : 1;
      const sugeridoB = this.esProductoSugerido(b) ? 0 : 1;
      return sugeridoA - sugeridoB || a.nombre.localeCompare(b.nombre);
    });

    if (!texto) return productos;
    return productos.filter(producto => producto.nombre.toLowerCase().includes(texto));
  }

  confirmarPedido(pedido: PedidoProveedor): void {
    const proveedor = this.proveedorSeleccionado();
    if (!proveedor || pedido.estado !== 'Pendiente') return;

    const pedidos = this._historialProveedor().map(item =>
      item.id === pedido.id ? { ...item, estado: 'Confirmado' as EstadoPedidoProveedor } : item
    );
    this._historialProveedor.set(pedidos);
    this.pedidoHistorialSeleccionado.update(seleccionado =>
      seleccionado?.id === pedido.id ? { ...seleccionado, estado: 'Confirmado' } : seleccionado
    );
    this.mensajeAccion.set('Pedido confirmado');
  }

  agregarIngredienteAPedido(pedido: PedidoProveedor, productoId: number | string, cantidad: number): void {
    const proveedor = this.proveedorSeleccionado();
    const producto = this.productos().find(item => item.id.toString() === productoId.toString());
    if (!proveedor || !producto || pedido.estado !== 'Pendiente' || cantidad <= 0) return;

    const item: PedidoProveedorItem = {
      id: producto.id,
      nombre: producto.nombre,
      cantidad,
      unidadMedida: producto.unidadMedida,
      precioUnitario: this.preciosMock[producto.id] ?? 500
    };

    const pedidos = this._historialProveedor().map(itemPedido => {
      if (itemPedido.id !== pedido.id) return itemPedido;

      const existe = itemPedido.items.some(pedidoItem => pedidoItem.id.toString() === item.id.toString());
      const items = existe
        ? itemPedido.items.map(pedidoItem => pedidoItem.id.toString() === item.id.toString()
          ? { ...pedidoItem, cantidad: pedidoItem.cantidad + item.cantidad, precioUnitario: item.precioUnitario }
          : pedidoItem)
        : [...itemPedido.items, item];
      const monto = items.reduce((total, pedidoItem) => total + (pedidoItem.precioUnitario ?? 0) * pedidoItem.cantidad, 0);

      return { ...itemPedido, items, monto };
    });

    this._historialProveedor.set(pedidos);
    this.pedidoHistorialSeleccionado.set(pedidos.find(itemPedido => itemPedido.id === pedido.id) ?? null);
    this.mensajeAccion.set('Ingrediente agregado');
  }

  seleccionarProducto(producto: Insumo): void {
    this.productoSeleccionadoId.set(producto.id);
    this.productoTexto.set(producto.nombre);
    this.cantidadProducto.set(this.getCantidadInicial(producto.unidadMedida));
    this.precioProductoManual.set(this.preciosMock[producto.id] ?? 500);
  }

  onProductoTextoChange(valor: string): void {
    this.productoTexto.set(valor);
    const valorNormalizado = valor.toLowerCase().trim();
    const encontrado = this.productos().find(producto => producto.nombre.toLowerCase() === valorNormalizado);

    if (encontrado) {
      this.productoSeleccionadoId.set(encontrado.id);
      this.cantidadProducto.set(this.getCantidadInicial(encontrado.unidadMedida));
      this.precioProductoManual.set(this.preciosMock[encontrado.id] ?? 500);
    } else {
      this.productoSeleccionadoId.set(null);
      this.precioProductoManual.set(null);
    }
  }

  agregarItemPedido(): void {
    const producto = this.productoBaseActual();
    const nombre = producto?.nombre ?? this.productoTexto().trim();
    const cantidad = this.cantidadProducto();
    const precio = producto ? (this.preciosMock[producto.id] ?? 500) : this.precioProductoManual();

    if (!nombre || cantidad === null || cantidad <= 0 || precio === null || precio <= 0) {
      return;
    }

    const unidadMedida = (producto?.unidadMedida as UnidadMedida) || 'UN';
    const itemId = producto?.id ?? `manual-${nombre.toLowerCase().replace(/\s+/g, '-')}`;

    this.pedidoItems.update(items => {
      const index = items.findIndex(item => item.id === itemId);
      if (index > -1) {
        return items.map(item => item.id === itemId ? { ...item, cantidad: item.cantidad + cantidad } : item);
      }
      return [...items, { id: itemId, nombre, cantidad, unidadMedida, precioUnitario: precio }];
    });

    this.productoTexto.set('');
    this.productoSeleccionadoId.set(null);
    this.cantidadProducto.set(1);
    this.precioProductoManual.set(null);
  }

  actualizarCantidadItem(itemId: string | number, cantidad: number | null): void {
    if (cantidad === null || cantidad <= 0) return;
    this.pedidoItems.update(items =>
      items.map(item => item.id === itemId ? { ...item, cantidad } : item)
    );
  }

  eliminarItemPedido(itemId: string | number): void {
    this.pedidoItems.update(items => items.filter(item => item.id !== itemId));
  }

  limpiarPedido(): void {
    this.pedidoItems.set([]);
    this.productoTexto.set('');
    this.productoSeleccionadoId.set(null);
    this.cantidadProducto.set(1);
    this.precioProductoManual.set(null);
    this.observacionPedido.set('');
    this.mensajeAccion.set(null);
  }

  enviarPedido(): void {
    const proveedor = this.proveedorSeleccionado();
    if (!proveedor || this.pedidoItems().length === 0) return;

    const observacion = this.observacionPedido().trim() || 'Pedido generado desde la vista de gerente';
    const concepto = 'Pedido de insumos';
    const items = [...this.pedidoItems()];
    const monto = this.montoEstimado();

    const fechaPedido = new Date().toISOString();
    const nuevoPedido: PedidoProveedor = {
      id: Date.now(),
      fecha: fechaPedido,
      concepto,
      monto,
      estado: 'Pendiente',
      observacion,
      items
    };

    const pedido = {
      proveedorId: proveedor.id,
      concepto: nuevoPedido.concepto,
      monto: nuevoPedido.monto,
      observacion: nuevoPedido.observacion,
      items: nuevoPedido.items
    };

    this.api.crearPedidoProveedor(proveedor.id, pedido)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (pedidoCreado) => {
          this._historialProveedor.update(pedidos => [pedidoCreado, ...pedidos]);
          this.proveedores.update(lista => lista.map(item => item.id === proveedor.id ? {
            ...item,
            fechaUltimoPedido: pedidoCreado.fecha
          } : item));
          this.proveedorSeleccionadoId.set(proveedor.id);
          this.panelModo.set('historial');
          
          this.abrirWhatsapp(proveedor, items, concepto, observacion);

          this.limpiarPedido();
          this.pedidoHistorialSeleccionado.set(null);
          this.mensajeAccion.set('Pedido agregado correctamente');
        }
      });
  }

  private abrirWhatsapp(proveedor: Proveedor, items: PedidoProveedorItem[], concepto: string, observacion: string): void {
    const telefonoLimpio = proveedor.telefono.replace(/[^0-9]/g, '');
    let mensaje = `Hola *${proveedor.nombre}*,\n\nTe realizo el siguiente pedido:\n`;
    mensaje += `*Concepto:* ${concepto}\n`;
    if (observacion && observacion !== 'Pedido generado desde la vista de gerente') {
      mensaje += `*Observaciones:* ${observacion}\n`;
    }
    mensaje += `\n*Detalle del pedido:*\n`;
    items.forEach(item => {
      mensaje += `- ${item.cantidad} ${item.unidadMedida} de *${item.nombre}*\n`;
    });

    const url = `https://wa.me/${telefonoLimpio}?text=${encodeURIComponent(mensaje)}`;
    window.open(url, '_blank');
  }

  private getCantidadConfiguracion(unidadMedida: UnidadMedida | string): { step: number; min: number; placeholder: string } {
    switch (unidadMedida) {
      case 'UN':
        return { step: 1, min: 1, placeholder: '1' };
      case 'GR':
        return { step: 10, min: 10, placeholder: '100' };
      case 'KG':
      case 'L':
      default:
        return { step: 0.1, min: 0.1, placeholder: '0.5' };
    }
  }


  private getCantidadInicial(unidadMedida: UnidadMedida | string): number {
    return this.getCantidadConfiguracion(unidadMedida).min;
  }

  private esProductoSugerido(producto: Insumo): boolean {
    const hoy = new Date();
    const vencimiento = new Date(`${producto.fechaVencimiento}T00:00:00`);
    const dias = Math.ceil((vencimiento.getTime() - hoy.getTime()) / 86400000);
    return producto.stock < producto.stockMinimo * 1.5 || dias <= 30;
  }
}
