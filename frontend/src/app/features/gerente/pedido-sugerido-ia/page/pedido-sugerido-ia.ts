import { Component, OnInit, signal, computed, inject, DestroyRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { ProveedorService } from '../../../../core/services/proveedor.service';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { ProductoStockMock } from '../../../../core/model/producto-stock-mock';

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
  private readonly proveedorService = inject(ProveedorService);
  private readonly destroyRef = inject(DestroyRef);

  proveedorId = signal<number>(0);
  proveedor = signal<Proveedor | null>(null);
  sugerencias = signal<SugerenciaPedidoItem[]>([]);
  pedidoItems = signal<SugerenciaPedidoItem[]>([]);
  
  observaciones = signal<string>('');
  busqueda = signal<string>('');
  productosDisponibles = signal<ProductoStockMock[]>([]);

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
      !this.pedidoItems().some(item => item.productoId === prod.id)
    );
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.proveedorId.set(+idParam);
    }
  }

  ngOnInit(): void {
    const id = this.proveedorId();
    if (isNaN(id) || id <= 0) {
      this.volver();
      return;
    }

    this.proveedorService.getProveedorById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(p => {
        if (p) {
          this.proveedor.set(p);
        } else {
          this.volver();
        }
      });

    this.proveedorService.getPedidoSugeridoIA(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(items => {
        this.sugerencias.set(items);
        this.pedidoItems.set(JSON.parse(JSON.stringify(items)));
      });

    this.proveedorService.getProductosDisponibles()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(prods => {
        this.productosDisponibles.set(prods);
      });
  }

  onSearchChanged(query: string): void {
    this.busqueda.set(query);
  }

  agregarProductoManual(prod: ProductoStockMock): void {
    const costosMock: Record<string, number> = {
      '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100,
      '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
    };

    const nuevo: SugerenciaPedidoItem = {
      productoId: prod.id,
      nombre: prod.nombre,
      unidadMedida: prod.unidadMedida,
      stockActual: prod.stock,
      stockMinimo: prod.stockMinimo,
      consumoEstimado30Dias: prod.stockMinimo * 3,
      cantidadSugerida: 1,
      precioUnitario: costosMock[prod.id] ?? 500
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

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  enviarPedido(): void {
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

    this.proveedorService.crearPedidoProveedor(prov.id, nuevoPedido)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], {
            state: { created: true, message: 'Pedido sugerido por IA enviado correctamente' }
          });
        },
        error: () => {
          // Manejo de errores
        }
      });
  }
}
