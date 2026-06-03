import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { forkJoin, of, switchMap } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { RealizarPedidoSugeridoApiService } from './realizar-pedido-sugerido.api';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { Insumo } from '../../../../core/models/insumos/insumo';

@Injectable({ providedIn: 'root' })
export class RealizarPedidoSugeridoStateService {
  private api = inject(RealizarPedidoSugeridoApiService);
  private destroyRef = inject(DestroyRef);

  // Estado centralizado expuesto como writeable signals
  proveedorId = signal<number>(0);
  proveedor = signal<Proveedor | null>(null);
  proveedores = signal<Proveedor[]>([]);
  sugerencias = signal<SugerenciaPedidoItem[]>([]);
  pedidoItems = signal<SugerenciaPedidoItem[]>([]);
  observaciones = signal<string>('');
  busqueda = signal<string>('');
  busquedaProveedor = signal<string>('');
  pedidosPorProveedor = signal<Record<string, SugerenciaPedidoItem[]>>({});
  observacionesPorProveedor = signal<Record<string, string>>({});
  mensajeError = signal<string | null>(null);
  
  private _loading = signal(false);
  loading = this._loading.asReadonly();

  // Variables Derivadas (Computed)
  montoEstimado = computed(() => {
    return this.pedidoItems().reduce((total, item) => total + (item.precioUnitario * item.cantidadSugerida), 0);
  });

  proveedoresFiltrados = computed(() => {
    const query = this.busquedaProveedor().toLowerCase().trim();
    const proveedores = this.proveedores();
    if (!query) return proveedores;
    return proveedores.filter(proveedor =>
      proveedor.nombre.toLowerCase().includes(query) ||
      proveedor.categorias?.some(categoria => categoria.toLowerCase().includes(query))
    );
  });

  // Métodos de Negocio
  cargarDatos(id?: number): void {
    this._loading.set(true);

    this.api.getProveedores()
      .pipe(
        switchMap(proveedores => {
          if (proveedores.length === 0) {
            return of({ proveedoresConItems: [], pedidosPorProveedor: {} as Record<string, SugerenciaPedidoItem[]> });
          }

          const consultas = proveedores.map(proveedor =>
            this.api.getInsumosAReponer(proveedor.id).pipe(
              map(items => ({ proveedor, items })),
              catchError(() => of({ proveedor, items: [] as SugerenciaPedidoItem[] }))
            )
          );

          return forkJoin(consultas).pipe(
            map(resultados => {
              const resultadosConItems = resultados.filter(resultado => resultado.items.length > 0);
              const proveedoresConItems = resultadosConItems.map(resultado => resultado.proveedor);
              const pedidosPorProveedor = resultadosConItems.reduce<Record<string, SugerenciaPedidoItem[]>>((acc, resultado) => {
                acc[resultado.proveedor.id.toString()] = JSON.parse(JSON.stringify(resultado.items));
                return acc;
              }, {});
              return { proveedoresConItems, pedidosPorProveedor };
            })
          );
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: ({ proveedoresConItems, pedidosPorProveedor }) => {
          this.proveedores.set(proveedoresConItems);
          const seleccionado =
            proveedoresConItems.find(proveedor => proveedor.id.toString() === id?.toString()) ??
            proveedoresConItems[0] ??
            null;
          this.proveedor.set(seleccionado);
          this.proveedorId.set(Number(seleccionado?.id ?? 0));
          this.pedidosPorProveedor.set(pedidosPorProveedor);
          this.sugerencias.set(seleccionado ? pedidosPorProveedor[seleccionado.id.toString()] ?? [] : []);
          this.pedidoItems.set(seleccionado ? pedidosPorProveedor[seleccionado.id.toString()] ?? [] : []);
          this._loading.set(false);
        },
        error: () => {
          this.proveedores.set([]);
          this.proveedor.set(null);
          this.proveedorId.set(0);
          this.pedidosPorProveedor.set({});
          this.sugerencias.set([]);
          this.pedidoItems.set([]);
          this._loading.set(false);
        }
      });
  }

  seleccionarProveedor(id: number | string): void {
    const proveedor = this.proveedores().find(item => item.id.toString() === id.toString()) ?? null;
    this.proveedor.set(proveedor);
    this.proveedorId.set(Number(proveedor?.id ?? 0));
    this.sugerencias.set([]);
    this.pedidoItems.set([]);
    this.busqueda.set('');
    this.mensajeError.set(null);

    if (!proveedor) return;

    this._loading.set(true);
    this.api.getInsumosAReponer(proveedor.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: items => {
          this.sugerencias.set(items);
          this.pedidoItems.set(JSON.parse(JSON.stringify(items)));
          this.pedidosPorProveedor.update(pedidos => ({
            ...pedidos,
            [proveedor.id.toString()]: JSON.parse(JSON.stringify(items))
          }));
          this._loading.set(false);
        },
        error: () => {
          this.sugerencias.set([]);
          this.pedidoItems.set([]);
          this.pedidosPorProveedor.update(pedidos => ({
            ...pedidos,
            [proveedor.id.toString()]: []
          }));
          this._loading.set(false);
        }
      });
  }

  setSearchTerm(query: string): void {
    this.busquedaProveedor.set(query);
  }

  eliminarItem(proveedorId: string | number, productoId: string): void {
    const key = proveedorId.toString();
    this.pedidosPorProveedor.update(pedidos => ({
      ...pedidos,
      [key]: (pedidos[key] ?? []).filter(item => item.productoId !== productoId)
    }));
  }

onCantidadCambiada(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    let cantidad = val ?? 1;
    if (cantidad <= 0) {
      cantidad = 1;
    }

    const key = proveedorId.toString();
    this.pedidosPorProveedor.update(pedidos => ({
      ...pedidos,
      [key]: (pedidos[key] ?? []).map(i =>
        i.productoId === item.productoId ? { ...i, cantidadSugerida: cantidad } : i
      )
    }));
  }

  onPrecioCambiado(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    const precio = val == null || val < 0 ? 0 : val;
    const key = proveedorId.toString();
    this.pedidosPorProveedor.update(pedidos => ({
      ...pedidos,
      [key]: (pedidos[key] ?? []).map(i =>
        i.productoId === item.productoId ? { ...i, precioUnitario: precio } : i
      )
    }));
  }

  setObservacionProveedor(proveedorId: string | number, observacion: string): void {
    this.observacionesPorProveedor.update(observaciones => ({
      ...observaciones,
      [proveedorId.toString()]: observacion
    }));
  }

  obtenerItemsProveedor(proveedorId: string | number): SugerenciaPedidoItem[] {
    return this.pedidosPorProveedor()[proveedorId.toString()] ?? [];
  }

  obtenerObservacionProveedor(proveedorId: string | number): string {
    return this.observacionesPorProveedor()[proveedorId.toString()] ?? '';
  }

  calcularMontoProveedor(proveedorId: string | number): number {
    return this.obtenerItemsProveedor(proveedorId)
      .reduce((total, item) => total + (item.precioUnitario * item.cantidadSugerida), 0);
  }

// ============================================================
  // TODO REFACTOR: bloque duplicado del panel "Agregar ingredientes"
  // copiado de historial-proveedor.ts para la demo del 03/06/2026.
  // Extraer a un componente compartido en shared/ui/ después de la entrega.
  // ============================================================
  productosDisponiblesPedido = signal<SugerenciaPedidoItem[]>([]);
  proveedorAgregarIngredienteId = signal<number | string | null>(null);
  busquedaIngrediente = signal('');
  productoExtraSeleccionadoId = signal<string>('');
  cantidadIngrediente = signal<number>(1);
  precioIngrediente = signal<number | null>(null);

  ingredientesParaAgregar = computed(() => {
    const proveedorId = this.proveedorAgregarIngredienteId();
    if (proveedorId === null) return [];

    const texto = this.busquedaIngrediente().toLowerCase().trim();
    const itemsActuales = this.obtenerItemsProveedor(proveedorId);
    const idsActuales = new Set(itemsActuales.map(i => i.productoId.toString()));

    return this.productosDisponiblesPedido()
      .filter(p => !idsActuales.has(p.productoId.toString()))
      .filter(p => !texto || p.nombre.toLowerCase().includes(texto));
  });

abrirAgregarIngrediente(proveedorId: number | string): void {
    this.proveedorAgregarIngredienteId.set(proveedorId);
    this.busquedaIngrediente.set('');
    this.productoExtraSeleccionadoId.set('');
    this.cantidadIngrediente.set(1);
    this.precioIngrediente.set(null);

    // Cargar TODOS los insumos disponibles (no solo los a reponer)
    this.api.getProductosDisponibles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (items: Insumo[]) => {
          const mapped: SugerenciaPedidoItem[] = items.map(i => ({
            productoId: i.id.toString(),
            nombre: i.nombre,
            unidadMedida: i.unidadMedida,
            stockActual: i.stockActual,
            stockMinimo: i.stockMinimo,
            cantidadSugerida: 1,
            precioUnitario: 0
          }));
          this.productosDisponiblesPedido.set(mapped);
        },
        error: () => this.productosDisponiblesPedido.set([])
      });
  }

  cerrarAgregarIngrediente(): void {
    this.proveedorAgregarIngredienteId.set(null);
  }

seleccionarIngredienteExtra(productoId: string): void {
    this.productoExtraSeleccionadoId.set(productoId);
    // Buscar el último precio en el historial del proveedor seleccionado
    const proveedorId = this.proveedorAgregarIngredienteId();
    if (proveedorId !== null) {
      this.api.getHistorialPedidos(proveedorId).subscribe({
        next: pedidos => {
          const ordenados = [...pedidos].sort((a, b) => {
            const fa = new Date(a.fecha).getTime();
            const fb = new Date(b.fecha).getTime();
            return fb - fa;
          });
          for (const pedido of ordenados) {
            const item = pedido.items.find(i => i.id.toString() === productoId);
            if (item && item.precioUnitario && item.precioUnitario > 0) {
              this.precioIngrediente.set(item.precioUnitario);
              return;
            }
          }
          this.precioIngrediente.set(null);
        },
        error: () => this.precioIngrediente.set(null)
      });
    }
  }

  setCantidadIngrediente(val: number | null): void {
    this.cantidadIngrediente.set(val == null || val <= 0 ? 1 : val);
  }

  setPrecioIngrediente(val: number | null): void {
    this.precioIngrediente.set(val == null || val < 0 ? null : val);
  }

  confirmarAgregarIngrediente(): void {
    const proveedorId = this.proveedorAgregarIngredienteId();
    const productoId = this.productoExtraSeleccionadoId();
    if (proveedorId === null || !productoId) return;

    const producto = this.productosDisponiblesPedido().find(p => p.productoId.toString() === productoId);
    if (!producto) return;

    const cantidad = this.cantidadIngrediente();
    const precio = this.precioIngrediente() ?? 0;
    const key = proveedorId.toString();

    const nuevoItem: SugerenciaPedidoItem = {
      ...producto,
      cantidadSugerida: cantidad,
      precioUnitario: precio
    };

    this.pedidosPorProveedor.update(pedidos => ({
      ...pedidos,
      [key]: [...(pedidos[key] ?? []), nuevoItem]
    }));

    this.cerrarAgregarIngrediente();
  }
  // ============================================================
  // FIN TODO REFACTOR
  // ============================================================

  enviarPedido(proveedor: Proveedor, onSuccess: () => void): void {
    this.mensajeError.set(null);
    const pedidoItems = this.obtenerItemsProveedor(proveedor.id);
    if (pedidoItems.length === 0) {
      this.mensajeError.set('No hay insumos seleccionados para crear este pedido.');
      return;
    }

    const items = pedidoItems.map(item => ({
      id: item.productoId,
      nombre: item.nombre,
      cantidad: item.cantidadSugerida,
      unidadMedida: item.unidadMedida,
      precioUnitario: item.precioUnitario
    }));

    const nuevoPedido = {
      proveedorId: proveedor.id,
      concepto: 'Pedido sugerido',
      monto: this.calcularMontoProveedor(proveedor.id),
      observacion: this.obtenerObservacionProveedor(proveedor.id).trim() || 'Pedido sugerido y revisado por el gerente.',
      items: items
    };

    this.api.crearPedidoProveedor(proveedor.id, nuevoPedido)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          onSuccess();
        },
        error: () => {
          this.mensajeError.set('No se pudo crear el pedido. Revisa las cantidades y vuelve a intentar.');
        }
      });
  }
}
