import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CategoriaInsumoService } from './categoria-insumo.service';
import { CategoriaInsumo } from '../../../../../core/models/domain/categoria-insumo';

@Injectable({
  providedIn: 'root',
})
export class CategoriaState {

  private api = inject(CategoriaInsumoService);
  private destroyRef = inject(DestroyRef);

  private _categorias = signal<CategoriaInsumo[]>([]);
  categorias = this._categorias.asReadonly();

  cargarCategorias(): void {
    this.api.obtenerCategorias().pipe(takeUntilDestroyed(this.destroyRef)).subscribe((categorias) => {
      this._categorias.set(categorias);
      void 0;
    });
  }

}
