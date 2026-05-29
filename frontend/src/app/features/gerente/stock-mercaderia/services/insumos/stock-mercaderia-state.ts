import { computed, inject, Injectable, signal } from '@angular/core';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { StockMercaderiaService } from './stock-mercaderia-service';

@Injectable({
  providedIn: 'root',
})
export class StockMercaderiaState {

  private api = inject(StockMercaderiaService);
 
  private _productos = signal<Insumo[]>([]);
  private _cargando = signal<boolean>(false);

  productos = this._productos.asReadonly();
  cargando = this._cargando.asReadonly();

  categoriasUnicas = computed(() =>{
    const list = this._productos();
    const unicas = new Set(list.map( p => p.categoria).filter(c=> Boolean(c)));

    console.log('Categorías únicas:', Array.from(unicas)); // Debug: Ver categorías únicas en consola
    return Array.from(unicas).sort();
  })

  productosCriticos = computed(() =>
    this._productos().filter(p => p.stockActual <= p.stockMinimo)
  );

  cantidadProductosCriticos = computed(() =>
    this.productosCriticos().length
  );

  cargarMercaderia(): void{
    this._cargando.set(true);
    this.api.getStockMercaderia().subscribe({
      next: (data) =>{
        this._productos.set(data);
        this._cargando.set(false);
       },
      error: (err) => {
        console.error('Error al cargar mercadería', err);
        this._cargando.set(false);
      }
    })
  }
  guardarProducto(producto: Insumo): void {
    this._cargando.set(true);
    
    if (producto.id) {
      // Caso: Edición
      this.api.actualizar(producto.id, producto).subscribe({
        next: (updated) => {
          this._productos.update(lista => 
            lista.map(p => p.id === updated.id ? updated : p)
          );
          this._cargando.set(false);
        },
        error: () => this._cargando.set(false)
      });
    } else {
      // Caso: Creación
      this.api.crear(producto).subscribe({
        next: (nuevo) => {
          this._productos.update(lista => [...lista, nuevo]);
          this._cargando.set(false);
        },
        error: () => this._cargando.set(false)
      });
    }
  }

  eliminarProducto(id: number): void {
    this._cargando.set(true);
    this.api.eliminar(id).subscribe({
      next: () => {
        this._productos.update(lista => lista.filter(p => p.id !== id));
        this._cargando.set(false);
      },
      error: () => this._cargando.set(false)
    });
  }

}
