import { Injectable, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProveedorApiService } from '../../services/proveedor.api';
import { ProveedorNuevo } from '../../../../core/models/domain/proveedor';
import { CategoriaInsumo } from '../../../../core/models/domain/categoria-insumo';

@Injectable({ providedIn: 'root' })
export class NuevoProveedorState {
  private api = inject(ProveedorApiService);
  private destroyRef = inject(DestroyRef);

  // Estado mutable expuesto como writeable signals
  categorias = signal<string[]>([]);
  categoriaIds = signal<number[]>([]);
  categoriasDisponibles = signal<CategoriaInsumo[]>([]);
  gerenteValidado = signal(false);
  mensajeErrorGerente = signal<string | null>(null);
  cargandoGerente = signal(false);
  errorCategorias = signal<string | null>(null);

  cargarCategorias(): void {
    this.errorCategorias.set(null);
    this.api.getCategoriasInsumo()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: categorias => this.categoriasDisponibles.set(categorias),
        error: () => this.errorCategorias.set('No pudimos cargar las categorías de insumo.')
      });
  }

 

  toggleCategoria(categoria: CategoriaInsumo): void {
    const idsActuales = [...this.categoriaIds()];
    const idx = idsActuales.indexOf(categoria.id);
    if (idx >= 0) {
      idsActuales.splice(idx, 1);
    } else {
      idsActuales.push(categoria.id);
    }
    this.categoriaIds.set(idsActuales);
    this.categorias.set(
      this.categoriasDisponibles()
        .filter(categoriaDisponible => idsActuales.includes(categoriaDisponible.id))
        .map(categoriaDisponible => categoriaDisponible.descripcion)
    );
  }

  removerCategoria(id: number): void {
    const actuales = this.categoriaIds().filter(categoriaId => categoriaId !== id);
    this.categoriaIds.set(actuales);
    this.categorias.set(
      this.categoriasDisponibles()
        .filter(categoriaDisponible => actuales.includes(categoriaDisponible.id))
        .map(categoriaDisponible => categoriaDisponible.descripcion)
    );
  }

  guardarProveedor(proveedor: ProveedorNuevo, onSuccess: () => void): void {
    this.api.crearProveedor(proveedor)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          onSuccess();
        }
      });
  }
}
