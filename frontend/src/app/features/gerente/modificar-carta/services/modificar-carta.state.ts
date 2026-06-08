import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ModificarCartaApiService } from './modificar-carta.api';
import { Plato } from '../../../../core/models/domain/plato';

@Injectable({ providedIn: 'root' })
export class ModificarCartaStateService {
  private api = inject(ModificarCartaApiService);
  private destroyRef = inject(DestroyRef);

  // 1. Estado PRIVADO
  readonly #searchTerm = signal<string>('');
  readonly #selectedCategoria = signal<string | null>(null);
  readonly #platos = signal<Plato[]>([]);
  readonly #platoAEditar = signal<Plato | null>(null);
  readonly #platoAEliminar = signal<Plato | null>(null);
  readonly #explodingPlatoId = signal<number | null>(null);
  readonly #loading = signal<boolean>(false);

  // 2. Estado PÚBLICO
  searchTerm = this.#searchTerm.asReadonly();
  selectedCategoria = this.#selectedCategoria.asReadonly();
  platos = this.#platos.asReadonly();
  platoAEditar = this.#platoAEditar.asReadonly();
  platoAEliminar = this.#platoAEliminar.asReadonly();
  explodingPlatoId = this.#explodingPlatoId.asReadonly();
  loading = this.#loading.asReadonly();

  // 3. Variables Derivadas (Computed)
  categoriasDisponibles = computed(() => {
    const cats = new Set(this.#platos().map(p => p.categoria).filter(c => !!c));
    return Array.from(cats).sort() as string[];
  });

  filteredPlatos = computed(() => {
    const sorted = [...this.#platos()].sort((a, b) => {
      if (a.visible === b.visible) return 0;
      return a.visible ? -1 : 1;
    });

    const lowerTerm = this.#searchTerm().toLowerCase().trim();
    const selectedCat = this.#selectedCategoria();

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
    this.#loading.set(true);
    this.api.getPlatos()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (platos) => {
          this.#platos.set(platos);
          this.#loading.set(false);
        },
        error: () => this.#loading.set(false)
      });
  }

  setSearchTerm(term: string): void {
    this.#searchTerm.set(term);
  }

  setCategoria(categoria: string | null): void {
    this.#selectedCategoria.set(categoria);
  }

  toggleVisibility(plato: Plato): void {
    if (plato.visible) {
      this.#explodingPlatoId.set(plato.id);

      setTimeout(() => {
        const targetState = false;
        this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));
        this.#explodingPlatoId.set(null);

        this.api.updatePlato(plato.id, { visible: targetState })
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: updated => {
              this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, ...updated } : p));
            },
            error: () => {
              this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: true } : p));
            }
          });
      }, 450);
    } else {
      const targetState = true;
      this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));

      this.api.updatePlato(plato.id, { visible: targetState })
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: updated => {
            this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, ...updated } : p));
          },
          error: () => {
            this.#platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: false } : p));
          }
        });
    }
  }

  setPlatoAEditar(plato: Plato | null): void {
    this.#platoAEditar.set(plato);
  }

  setPlatoAEliminar(plato: Plato | null): void {
    this.#platoAEliminar.set(plato);
  }

  savePlato(updatedFields: Partial<Plato>): void {
    const target = this.#platoAEditar();
    if (!target) return;

    this.api.updatePlato(target.id, updatedFields)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(updated => {
        this.#platos.update(platos => platos.map(p => p.id === target.id ? { ...p, ...updated } : p));
        this.#platoAEditar.set(null);
      });
  }

  confirmDelete(): void {
    const target = this.#platoAEliminar();
    if (!target) return;

    this.api.deletePlato(target.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.#platos.update(platos => platos.filter(p => p.id !== target.id));
        this.#platoAEliminar.set(null);
      });
  }

  closeModals(): void {
    this.#platoAEditar.set(null);
    this.#platoAEliminar.set(null);
  }

  toggleRecomendado(plato: Plato): void {
    const targetState = !plato.recomendado;
    // Optimistic update
    this.#platos.update(platos =>
      platos.map(p => p.id === plato.id ? { ...p, recomendado: targetState } : p)
    );

    this.api.updatePlato(plato.id, { recomendado: targetState })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this.#platos.update(platos =>
            platos.map(p => p.id === plato.id ? { ...p, ...updated } : p)
          );
        },
        error: () => {
          // Revert on error
          this.#platos.update(platos =>
            platos.map(p => p.id === plato.id ? { ...p, recomendado: plato.recomendado } : p)
          );
        }
      });
  }
}
