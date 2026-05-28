import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PedidoSugeridoIAApiService } from './pedido-sugerido-ia.api';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';

@Injectable({ providedIn: 'root' })
export class PedidoSugeridoIAStateService {
  private api = inject(PedidoSugeridoIAApiService);
  private destroyRef = inject(DestroyRef);

  // Estado centralizado expuesto como writeable signals
  proveedorId = signal<number>(0);
  proveedor = signal<Proveedor | null>(null);
  sugerencias = signal<SugerenciaPedidoItem[]>([]);
  pedidoItems = signal<SugerenciaPedidoItem[]>([]);
  observaciones = signal<string>('');
  busqueda = signal<string>('');
  productosDisponibles = signal<ProductoStockMock[]>([]);
  
  private _loading = signal(false);
  loading = this._loading.asReadonly();

  // Variables Derivadas (Computed)
  montoEstimado = computed(() => {
    return this.pedidoItems().reduce((total, item) => total + (item.precioUnitario * item.cantidadSugerida), 0);
  });

  sugerenciasExtras = computed(() => {
    const query = this.busqueda().toLowerCase().trim();
    if (!query) return [];

    const prov = this.proveedor();
    const categories = prov?.categorias ?? [];

    return this.productosDisponibles().filter(prod =>
      prod.nombre.toLowerCase().includes(query) &&
      categories.includes(prod.categoriaIngrediente) &&
      !this.pedidoItems().some(item => item.productoId === prod.id.toString())
    );
  });

  // Métodos de Negocio
  cargarDatos(id: number, onBack: () => void): void {
    if (isNaN(id) || id <= 0) {
      onBack();
      return;
    }

    this._loading.set(true);

    this.api.getProveedorById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(p => {
        if (p) {
          this.proveedor.set(p);
        } else {
          onBack();
        }
      });

    this.api.getPedidoSugeridoIA(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(items => {
        this.sugerencias.set(items);
        this.pedidoItems.set(JSON.parse(JSON.stringify(items)));
        this._loading.set(false);
      });

    this.api.getProductosDisponibles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(prods => {
        this.productosDisponibles.set(prods);
      });
  }

  setSearchTerm(query: string): void {
    this.busqueda.set(query);
  }

  agregarProductoManual(prod: ProductoStockMock): void {
    const costosMock: Record<string, number> = {
      '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100,
      '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
    };

    const nuevo: SugerenciaPedidoItem = {
      productoId: prod.id.toString(),
      nombre: prod.nombre,
      unidadMedida: prod.unidadMedida,
      stockActual: prod.stock,
      stockMinimo: prod.stockMinimo,
      consumoEstimado30Dias: prod.stockMinimo * 3,
      cantidadSugerida: 1,
      precioUnitario: costosMock[prod.id.toString()] ?? 500
    };

    this.pedidoItems.update(items => [...items, nuevo]);
    this.busqueda.set('');
  }

  eliminarItem(productoId: string): void {
    this.pedidoItems.update(items => items.filter(item => item.productoId !== productoId));
  }

  onCantidadCambiada(item: SugerenciaPedidoItem, val: number | null): void {
    let cantidad = val ?? 1;
    if (cantidad <= 0) {
      cantidad = 1;
    }

    this.pedidoItems.update(items =>
      items.map(i => i.productoId === item.productoId ? { ...i, cantidadSugerida: cantidad } : i)
    );
  }

  enviarPedido(onSuccess: () => void): void {
    const prov = this.proveedor();
    if (!prov || this.pedidoItems().length === 0) return;

    const items = this.pedidoItems().map(item => ({
      id: item.productoId,
      nombre: item.nombre,
      cantidad: item.cantidadSugerida,
      unidadMedida: item.unidadMedida,
      precioUnitario: item.precioUnitario
    }));

    const nuevoPedido = {
      proveedorId: prov.id,
      concepto: 'Pedido Sugerido por IA',
      monto: this.montoEstimado(),
      observacion: this.observaciones().trim() || 'Pedido sugerido por IA y revisado por el gerente.',
      items: items
    };

    this.api.crearPedidoProveedor(prov.id, nuevoPedido)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          onSuccess();
        }
      });
  }
}
