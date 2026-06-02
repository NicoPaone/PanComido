import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { forkJoin, of, switchMap } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { RealizarPedidoSugeridoApiService } from './realizar-pedido-sugerido.api';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';

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
