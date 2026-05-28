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
  private _platos = signal<Plato[]>([]);
  private _platoAEditar = signal<Plato | null>(null);
  private _platoAEliminar = signal<Plato | null>(null);
  private _explodingPlatoId = signal<number | null>(null);
  private _loading = signal<boolean>(false);

  // 2. Estado PÚBLICO
  searchTerm = this._searchTerm.asReadonly();
  platos = this._platos.asReadonly();
  platoAEditar = this._platoAEditar.asReadonly();
  platoAEliminar = this._platoAEliminar.asReadonly();
  explodingPlatoId = this._explodingPlatoId.asReadonly();
  loading = this._loading.asReadonly();

  // 3. Variables Derivadas (Computed)
  filteredPlatos = computed(() => {
    const sorted = [...this._platos()].sort((a, b) => {
      if (a.visible === b.visible) return 0;
      return a.visible ? -1 : 1;
    });

    const lowerTerm = this._searchTerm().toLowerCase().trim();
    if (!lowerTerm) {
      return sorted;
    }
    return sorted.filter(plato => 
      plato.nombre.toLowerCase().includes(lowerTerm)
    );
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
              this._platos.update(platos => platos.map(p => p.id === plato.id ? updated : p));
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
            this._platos.update(platos => platos.map(p => p.id === plato.id ? updated : p));
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
        this._platos.update(platos => platos.map(p => p.id === target.id ? updated : p));
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
}
