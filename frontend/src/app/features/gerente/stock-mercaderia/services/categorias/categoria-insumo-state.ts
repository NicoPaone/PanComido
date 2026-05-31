import { inject, Injectable, signal } from '@angular/core';
import { CategoriaInsumoService } from './categoria-insumo.service';
import { CategoriaInsumo } from '../../../../../core/models/insumos/categorias/categoria-insumo';

@Injectable({
  providedIn: 'root',
})
export class CategoriaState {

  private api = inject(CategoriaInsumoService);

  private _categorias = signal<CategoriaInsumo[]>([]);
  categorias = this._categorias.asReadonly();

  cargarCategorias(): void {
    this.api.obtenerCategorias().subscribe((categorias) => {
      this._categorias.set(categorias);
    });
  }

}
