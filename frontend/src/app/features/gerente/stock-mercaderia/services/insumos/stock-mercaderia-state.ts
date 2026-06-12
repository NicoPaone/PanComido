import { computed, DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Insumo, CrearInsumo, LoteInsumo } from '../../../../../core/models/domain/insumo';
import { StockMercaderiaService } from './stock-mercaderia-service';
import { UnidadMedidaService } from '../unidad-medida.service';
import { CategoriaInsumoService } from '../categorias/categoria-insumo.service';
import { UnidadMedida } from '../../../../../core/models/domain/unidad-medida';
import { CategoriaInsumo } from '../../../../../core/models/domain/categoria-insumo';
import { forkJoin } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class StockMercaderiaState {
  private api = inject(StockMercaderiaService);
  private apiUnidadMedida = inject(UnidadMedidaService);
  private apiCategoriaInsumos = inject(CategoriaInsumoService);
  private destroyRef = inject(DestroyRef);
  
  readonly #productos = signal<Insumo[]>([]);
  readonly #lotes = signal<LoteInsumo[]>([]);
  readonly #lotesCargados = signal<boolean>(false);
  readonly #unidadMedidas = signal<UnidadMedida[]>([]);
  readonly #categoriasInsumos = signal<CategoriaInsumo[]>([]);
  readonly #cargando = signal<boolean>(false);

  productos = this.#productos.asReadonly();
  lotes = this.#lotes.asReadonly();
  lotesCargados = this.#lotesCargados.asReadonly();
  cargando = this.#cargando.asReadonly();
  unidadMedidas = this.#unidadMedidas.asReadonly();
  categoriasInsumos = this.#categoriasInsumos.asReadonly();

  productosCriticos = computed(() =>
    this.#productos().filter(p => p.stockActual <= p.stockMinimo)
  );

  cantidadProductosCriticos = computed(() =>
    this.productosCriticos().length
  );

  cargarMercaderia(): void {
    this.#cargando.set(true);
    this.api.getStockMercaderia().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this.#productos.set(data);
        this.#cargando.set(false);
      },
      error: (err) => {
        
        this.#cargando.set(false);
      }
    });
  }

  cargarLotes(): void {
    if (this.#lotesCargados()) return;

    this.api.getLotes().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this.#lotes.set(data);
        this.#lotesCargados.set(true);
      },
      error: () => {
        this.#lotes.set([]);
        this.#lotesCargados.set(true);
      }
    });
  }

  cargarCatalogos(): void { 
    forkJoin({
      categoriasRes: this.apiCategoriaInsumos.obtenerCategorias(),
      unidadesRes: this.apiUnidadMedida.obtenerUnidades()
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (response) => {
        this.#categoriasInsumos.set(response.categoriasRes);
        this.#unidadMedidas.set(response.unidadesRes);
      },
      error: (err) => {}
    });
  }

  guardarProducto(producto: CrearInsumo): void {
    this.#cargando.set(true);
    
    const idEdicion = 'id' in producto ? (producto as any).id : null;

    if (idEdicion) {
      this.api.actualizar(idEdicion, producto).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: (updated) => {
          this.#productos.update(lista => 
            lista.map(p => p.id === updated.id ? updated : p)
          );
          this.#cargando.set(false);
        },
        error: () => this.#cargando.set(false)
      });
    } else {
      this.api.crear(producto).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: (nuevo: Insumo) => {
          const lotesEstabanCargados = this.#lotesCargados();
          this.#productos.update(lista => [...lista, nuevo]);
          this.#lotesCargados.set(false);
          if (lotesEstabanCargados) this.cargarLotes();
          this.#cargando.set(false);
          
        },
        error: (err) => {
          this.#cargando.set(false)
          

        }
      });
    }
  }

  eliminarProducto(id: number): void {
    this.#cargando.set(true);
    this.api.eliminar(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.#productos.update(lista => lista.filter(p => p.id !== id));
        this.#cargando.set(false);
      },
      error: () => this.#cargando.set(false)
    });
  }
}
