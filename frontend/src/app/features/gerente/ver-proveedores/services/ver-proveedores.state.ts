import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { VerProveedoresApiService } from './ver-proveedores.api';
import { Proveedor, PedidoProveedor, PedidoProveedorItem } from '../../../../core/models/domain/proveedor';
import { RecepcionPedidoItem } from '../../../../core/models/domain/proveedor';
import { Insumo } from '../../../../core/models/domain/insumo';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { Bodega } from '../../../../core/models/domain/bodega';

@Injectable({ providedIn: 'root' })
export class VerProveedoresState {
  private api = inject(VerProveedoresApiService);
  private destroyRef = inject(DestroyRef);

  /*private readonly preciosMock: Record<string, number> = {
    '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100,
    '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
  };*/

  termino = signal('');
  filtroEstado = signal<'Todos' | 'Activos' | 'Inactivos'>('Todos');
  proveedores = signal<Proveedor[]>([]);
  productos = signal<Insumo[]>([]);
  proveedorSeleccionadoId = signal<number | string | null>(null);
  panelModo = signal<'pedido' | 'historial'>('historial');
  observacionPedido = signal('');
  mensajeAccion = signal<string | null>(null);
  productoTexto = signal('');
  productoSeleccionadoId = signal<string | number | null>(null);
  cantidadProducto = signal<number | null>(1);
  precioProductoManual = signal<number | null>(null);
  pedidoItems = signal<PedidoProveedorItem[]>([]);
  pedidoHistorialSeleccionado = signal<PedidoProveedor | null>(null);
  recepcionPedido = signal<PedidoProveedor | null>(null);
  recepcionItems = signal<RecepcionPedidoItem[]>([]);
  bodegas = signal<Bodega[]>([]);

  private _loading = signal(false);
  loading = this._loading.asReadonly();
  private _error = signal<string | null>(null);
  error = this._error.asReadonly();

  private _historialProveedor = signal<PedidoProveedor[]>([]);
  historialProveedor = this._historialProveedor.asReadonly();

  private _loadingHistorial = signal(false);
  loadingHistorial = this._loadingHistorial.asReadonly();
  private _errorHistorial = signal<string | null>(null);
  errorHistorial = this._errorHistorial.asReadonly();

  private _loadingInsumos = signal(false);
  loadingInsumos = this._loadingInsumos.asReadonly();
  private _errorInsumos = signal<string | null>(null);
  errorInsumos = this._errorInsumos.asReadonly();

  proveedoresFiltrados = computed(() => {
    const texto = this.termino().toLowerCase().trim();
    const filtro = this.filtroEstado();

    const lista = [...this.proveedores()].filter(prov => {
      if (filtro === 'Activos' && !prov.activo) return false;
      if (filtro === 'Inactivos' && prov.activo) return false;
      return true;
    }).sort((a, b) => {
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

  pedidosListosParaRecibir = computed(() => {
    return this._historialProveedor().filter(pedido => pedido.estado === 'Enviado');
  });

 montoEstimado = computed(() => {
    return this.pedidoItems().reduce((total, item) => {
      const base = item.precioUnitario ?? 0;
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
    this._error.set(null);
    this.api.getProveedores()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (provs) => {
          this.proveedores.set(provs);
          if (provs.length > 0 && this.proveedorSeleccionadoId() === null) {
            this.proveedorSeleccionadoId.set(provs[0].id);
            this.cargarHistorial(provs[0].id);
            this.cargarInsumosProveedor(provs[0].id);
          }
          this._loading.set(false);
        },
        error: () => {
          this.proveedores.set([]);
          this.proveedorSeleccionadoId.set(null);
          this._historialProveedor.set([]);
          this.productos.set([]);
          this._error.set('No pudimos cargar los proveedores. Revisá la conexión e intentá nuevamente.');
          this._loading.set(false);
        }
      });

    this.api.getBodegas()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(bodegas => {
        this.bodegas.set(bodegas);
      });
  }

  /**
   * Carga solo la lista de proveedores y bodegas, sin auto-seleccionar ni cargar insumos.
   * Usar desde vistas que ya gestionan la selección del proveedor (ej: historial-proveedor).
   */
  cargarProveedoresSolos(): void {
    this._loading.set(true);
    this._error.set(null);
    this.api.getProveedores()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (provs) => {
          this.proveedores.set(provs);
          this._loading.set(false);
        },
        error: () => {
          this.proveedores.set([]);
          this._error.set('No pudimos cargar los proveedores. Revisá la conexión e intentá nuevamente.');
          this._loading.set(false);
        }
      });

    this.api.getBodegas()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(bodegas => {
        this.bodegas.set(bodegas);
      });
  }

  seleccionarProveedor(proveedorId: number | string): void {
    this.proveedorSeleccionadoId.set(proveedorId);
    this.mensajeAccion.set(null);
    this.pedidoHistorialSeleccionado.set(null);
    this._historialProveedor.set([]);
    this.productoTexto.set('');
    this.productoSeleccionadoId.set(null);
    this.productos.set([]);
    this._errorHistorial.set(null);
    this._errorInsumos.set(null);
    this.cargarHistorial(proveedorId);
    this.cargarInsumosProveedor(proveedorId);
  }

  cargarInsumosProveedor(proveedorId: number | string): void {
    this._loadingInsumos.set(true);
    this._errorInsumos.set(null);
    this.api.getInsumosProveedor(proveedorId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (productos) => {
          this.productos.set(productos.filter(producto => producto.nombre?.trim()));
          this._loadingInsumos.set(false);
        },
        error: () => {
          this.productos.set([]);
          this._errorInsumos.set('No pudimos cargar los insumos de este proveedor.');
          this._loadingInsumos.set(false);
        }
      });
  }

  cargarHistorial(id: number | string): void {
    this._loadingHistorial.set(true);
    this._errorHistorial.set(null);
    this.api.getHistorialPedidos(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (pedidos) => {
          this._historialProveedor.set(pedidos);
          this._loadingHistorial.set(false);
        },
        error: () => {
          this._historialProveedor.set([]);
          this._errorHistorial.set('No pudimos cargar el historial de este proveedor.');
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
    const vistos = new Set<string>();
    const productos = [...this.productos()]
      .filter(producto => {
        const nombre = producto.nombre?.trim();
        const id = producto.id?.toString();
        if (!nombre || !id || vistos.has(id)) return false;
        vistos.add(id);
        return !this.pedidoHistorialSeleccionado()?.items.some(item => item.id.toString() === id);
      })
      .sort((a, b) => {
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

    this.api.confirmarPedido(pedido)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ pedido: pedidoConfirmado, linkWpp }) => {
          this._historialProveedor.update(pedidos =>
            pedidos.map(item => item.id === pedido.id ? pedidoConfirmado : item)
          );
          this.pedidoHistorialSeleccionado.update(seleccionado =>
            seleccionado?.id === pedido.id ? pedidoConfirmado : seleccionado
          );
          this.mensajeAccion.set('Pedido enviado al proveedor');
          window.open(linkWpp, '_blank');
        }
      });
  }

agregarIngredienteAPedido(pedido: PedidoProveedor, productoId: number | string, cantidad: number, precio: number): void {    const proveedor = this.proveedorSeleccionado();
    const producto = this.productos().find(item => item.id.toString() === productoId.toString());
    if (!proveedor || !producto || pedido.estado !== 'Pendiente' || cantidad <= 0) return;

    const item: PedidoProveedorItem = {
      id: producto.id,
      nombre: producto.nombre,
      cantidad,
      unidadMedida: producto.unidadMedida,
      precioUnitario: precio
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
    this.precioProductoManual.set(this.ultimoPrecioDeInsumo(producto.id));
  }

  private ultimoPrecioDeInsumo(insumoId: number | string): number | null {
    // Busca en el historial el último pedido (ordenado por fecha desc) que tenga este insumo
    // y devuelve el precio de compra que se pagó en ese pedido.
    const historial = [...this._historialProveedor()].sort((a, b) => {
      const fa = new Date(a.fecha).getTime();
      const fb = new Date(b.fecha).getTime();
      return fb - fa;
    });

    for (const pedido of historial) {
      const item = pedido.items.find(i => i.id.toString() === insumoId.toString());
      if (item && item.precioUnitario && item.precioUnitario > 0) {
        return item.precioUnitario;
      }
    }
    return null;
  }

  onProductoTextoChange(valor: string): void {
    this.productoTexto.set(valor);
    const valorNormalizado = valor.toLowerCase().trim();
    const encontrado = this.productos().find(producto => producto.nombre.toLowerCase() === valorNormalizado);

    if (encontrado) {
      this.productoSeleccionadoId.set(encontrado.id);
      this.cantidadProducto.set(this.getCantidadInicial(encontrado.unidadMedida));
      this.precioProductoManual.set(this.ultimoPrecioDeInsumo(encontrado.id));
    } else {
      this.productoSeleccionadoId.set(null);
      this.precioProductoManual.set(null);
    }
  }

  agregarItemPedido(): void {
    const producto = this.productoBaseActual();
    const nombre = producto?.nombre ?? this.productoTexto().trim();
    const cantidad = this.cantidadProducto();
    const precio = this.precioProductoManual();

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

    const pedido = {
      proveedorId: proveedor.id,
      concepto,
      monto,
      observacion,
      items
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
          this.limpiarPedido();
          this.pedidoHistorialSeleccionado.set(null);
          this.mensajeAccion.set('Pedido creado correctamente. Quedó pendiente en el historial.');
        }
      });
  }

  previsualizarRecepcion(pedido: PedidoProveedor): void {
    if (pedido.estado !== 'Enviado') return;
    this.recepcionPedido.set(pedido);
    this.api.previsualizarConfirmacion(pedido.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(items => {
        this.recepcionItems.set(items);
      });
  }

  cerrarRecepcion(): void {
    this.recepcionPedido.set(null);
    this.recepcionItems.set([]);
  }

  actualizarRecepcionItem(insumoId: number, cambios: Partial<RecepcionPedidoItem>): void {
    this.recepcionItems.update(items =>
      items.map(item => item.insumoId === insumoId ? { ...item, ...cambios } : item)
    );
  }

  recibirPedido(): void {
    const pedido = this.recepcionPedido();
    const items = this.recepcionItems();
    if (!pedido || items.length === 0) return;

    this.api.recibirPedido(pedido.id, items)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          const recibido: PedidoProveedor = { ...pedido, estado: 'Recibido' };
          this._historialProveedor.update(pedidos =>
            pedidos.map(item => item.id === pedido.id ? recibido : item)
          );
          this.pedidoHistorialSeleccionado.update(seleccionado =>
            seleccionado?.id === pedido.id ? recibido : seleccionado
          );
          this.cerrarRecepcion();
          this.mensajeAccion.set('El pedido se recibió y se guardó en el stock correctamente.');
        }
      });
  }

  private getCantidadConfiguracion(unidadMedida: UnidadMedida | string): { step: number; min: number; placeholder: string } {
    const nombreUnidad = typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
    switch (nombreUnidad.trim().toUpperCase()) {
      case 'UNIDAD':
      case 'UNIDADES':
      case 'UN':
      case 'PORCION':
      case 'PORCIONES':
        return { step: 1, min: 1, placeholder: '1' };
      case 'GR':
      case 'GRAMO':
      case 'GRAMOS':
        return { step: 10, min: 10, placeholder: '100' };
      case 'KILO':
      case 'KILOS':
      case 'KG':
        return { step: 0.1, min: 0.1, placeholder: '0.5' };
      case 'LITRO':
      case 'LITROS':
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
    const vencimiento = new Date(`${producto.vencimiento}T00:00:00`);
    const dias = Math.ceil((vencimiento.getTime() - hoy.getTime()) / 86400000);
    return producto.stockActual < producto.stockMinimo * 1.5 || dias <= 30;
  }
}
