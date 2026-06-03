import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ModificarCartaApiService } from './modificar-carta.api';
import { Plato } from '../../../../core/models/plato';

@Injectable({ providedIn: 'root' })
export class ModificarCartaStateService {
  private api = inject(ModificarCartaApiService);
  private destroyRef = inject(DestroyRef);

  // 1. Estado PRIVADO
  private _searchTerm = signal<string>('');
  private _selectedCategoria = signal<string | null>(null);
  private _platos = signal<Plato[]>([]);
  private _platoAEditar = signal<Plato | null>(null);
  private _platoAEliminar = signal<Plato | null>(null);
  private _explodingPlatoId = signal<number | null>(null);
  private _loading = signal<boolean>(false);

  // 2. Estado PÚBLICO
  searchTerm = this._searchTerm.asReadonly();
  selectedCategoria = this._selectedCategoria.asReadonly();
  platos = this._platos.asReadonly();
  platoAEditar = this._platoAEditar.asReadonly();
  platoAEliminar = this._platoAEliminar.asReadonly();
  explodingPlatoId = this._explodingPlatoId.asReadonly();
  loading = this._loading.asReadonly();

  // 3. Variables Derivadas (Computed)
  categoriasDisponibles = computed(() => {
    const cats = new Set(this._platos().map(p => p.categoria).filter(c => !!c));
    return Array.from(cats).sort() as string[];
  });

  filteredPlatos = computed(() => {
    const sorted = [...this._platos()].sort((a, b) => {
      if (a.visible === b.visible) return 0;
      return a.visible ? -1 : 1;
    });

    const lowerTerm = this._searchTerm().toLowerCase().trim();
    const selectedCat = this._selectedCategoria();

    let result = sorted;
    if (lowerTerm) {
      result = result.filter(plato => 
        plato.nombre.toLowerCase().includes(lowerTerm)
      );
    }
    if (selectedCat) {
      result = result.filter(plato => 
        plato.categoria === selectedCat
      );
    }
    return result;
  });

  platosRecomendados = computed(() => {
    return this.filteredPlatos()
      .filter(plato => plato.recomendado && plato.visible)
      .sort((a, b) => (b.ventas ?? 0) - (a.ventas ?? 0));
  });

  platosNormales = computed(() => {
    const normal = this.filteredPlatos().filter(plato => !(plato.recomendado && plato.visible));
    return [...normal].sort((a, b) => {
      const aVisible = a.visible ?? true;
      const bVisible = b.visible ?? true;
      if (aVisible !== bVisible) {
        return aVisible ? -1 : 1;
      }
      if (!aVisible) {
        const aRec = !!a.recomendado;
        const bRec = !!b.recomendado;
        if (aRec && !bRec) return 1;
        if (!aRec && bRec) return -1;
      }
      return 0;
    });
  });

  // 4. Métodos de Negocio (UseCases)
  cargarPlatos(): void {
    this._loading.set(true);
    this.api.getPlatos()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (platos) => {
          this._platos.set(platos);
          this._loading.set(false);
        },
        error: () => this._loading.set(false)
      });
  }

  setSearchTerm(term: string): void {
    this._searchTerm.set(term);
  }

  setCategoria(categoria: string | null): void {
    this._selectedCategoria.set(categoria);
  }

  toggleVisibility(plato: Plato): void {
    if (plato.visible) {
      this._explodingPlatoId.set(plato.id);

      setTimeout(() => {
        const targetState = false;
        this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));
        this._explodingPlatoId.set(null);

        this.api.updatePlato(plato.id, { visible: targetState })
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: updated => {
              this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, ...updated } : p));
            },
            error: () => {
              this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: true } : p));
            }
          });
      }, 450);
    } else {
      const targetState = true;
      this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));

      this.api.updatePlato(plato.id, { visible: targetState })
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: updated => {
            this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, ...updated } : p));
          },
          error: () => {
            this._platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: false } : p));
          }
        });
    }
  }

  setPlatoAEditar(plato: Plato | null): void {
    this._platoAEditar.set(plato);
  }

  setPlatoAEliminar(plato: Plato | null): void {
    this._platoAEliminar.set(plato);
  }

  savePlato(updatedFields: Partial<Plato>): void {
    const target = this._platoAEditar();
    if (!target) return;

    this.api.updatePlato(target.id, updatedFields)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(updated => {
        this._platos.update(platos => platos.map(p => p.id === target.id ? { ...p, ...updated } : p));
        this._platoAEditar.set(null);
      });
  }

  confirmDelete(): void {
    const target = this._platoAEliminar();
    if (!target) return;

    this.api.deletePlato(target.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this._platos.update(platos => platos.filter(p => p.id !== target.id));
        this._platoAEliminar.set(null);
      });
  }

  closeModals(): void {
    this._platoAEditar.set(null);
    this._platoAEliminar.set(null);
  }

  toggleRecomendado(plato: Plato): void {
    const targetState = !plato.recomendado;
    // Optimistic update
    this._platos.update(platos =>
      platos.map(p => p.id === plato.id ? { ...p, recomendado: targetState } : p)
    );

    this.api.updatePlato(plato.id, { recomendado: targetState })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this._platos.update(platos =>
            platos.map(p => p.id === plato.id ? { ...p, ...updated } : p)
          );
        },
        error: () => {
          // Revert on error
          this._platos.update(platos =>
            platos.map(p => p.id === plato.id ? { ...p, recomendado: plato.recomendado } : p)
          );
        }
      });
  }
}
