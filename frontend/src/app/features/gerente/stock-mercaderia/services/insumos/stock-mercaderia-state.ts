import { computed, DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Insumo, CrearInsumo } from '../../../../../core/models/domain/insumo';
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
  
  private _productos = signal<Insumo[]>([]);
  private _unidadMedidas = signal<UnidadMedida[]>([]);
  private _categoriasInsumos = signal<CategoriaInsumo[]>([]);
  private _cargando = signal<boolean>(false);

  productos = this._productos.asReadonly();
  cargando = this._cargando.asReadonly();
  unidadMedidas = this._unidadMedidas.asReadonly();
  categoriasInsumos = this._categoriasInsumos.asReadonly();

  productosCriticos = computed(() =>
    this._productos().filter(p => p.stockActual <= p.stockMinimo)
  );

  cantidadProductosCriticos = computed(() =>
    this.productosCriticos().length
  );

  cargarMercaderia(): void {
    this._cargando.set(true);
    this.api.getStockMercaderia().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this._productos.set(data);
        this._cargando.set(false);
      },
      error: (err) => {
        
        this._cargando.set(false);
      }
    });
  }

  cargarCatalogos(): void { 
    forkJoin({
      categoriasRes: this.apiCategoriaInsumos.obtenerCategorias(),
      unidadesRes: this.apiUnidadMedida.obtenerUnidades()
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (response) => {
        this._categoriasInsumos.set(response.categoriasRes);
        this._unidadMedidas.set(response.unidadesRes);
      },
      error: (err) => {}
    });
  }

  guardarProducto(producto: CrearInsumo): void {
    this._cargando.set(true);
    
    const idEdicion = 'id' in producto ? (producto as any).id : null;

    if (idEdicion) {
      this.api.actualizar(idEdicion, producto).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: (updated) => {
          this._productos.update(lista => 
            lista.map(p => p.id === updated.id ? updated : p)
          );
          this._cargando.set(false);
        },
        error: () => this._cargando.set(false)
      });
    } else {
      this.api.crear(producto).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
        next: (nuevo: Insumo) => {
          this._productos.update(lista => [...lista, nuevo]);
          this._cargando.set(false);
          
        },
        error: (err) => {
          this._cargando.set(false)
          

        }
      });
    }
  }

  eliminarProducto(id: number): void {
    this._cargando.set(true);
    this.api.eliminar(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this._productos.update(lista => lista.filter(p => p.id !== id));
        this._cargando.set(false);
      },
      error: () => this._cargando.set(false)
    });
  }
}