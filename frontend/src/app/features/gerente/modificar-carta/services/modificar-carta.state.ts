import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ModificarCartaApiService } from './modificar-carta.api';
import { Plato } from '../../../../core/models/domain/plato';

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
  private _selectedTipoBebida = signal<string | null>(null);
  private _selectedTipoComida = signal<string | null>(null);
  private _sortOrder = signal<'default' | 'ventas-desc' | 'ventas-asc'>('default');

  // 2. Estado PÚBLICO
  searchTerm = this._searchTerm.asReadonly();
  selectedCategoria = this._selectedCategoria.asReadonly();
  platos = this._platos.asReadonly();
  platoAEditar = this._platoAEditar.asReadonly();
  platoAEliminar = this._platoAEliminar.asReadonly();
  explodingPlatoId = this._explodingPlatoId.asReadonly();
  loading = this._loading.asReadonly();
  selectedTipoBebida = this._selectedTipoBebida.asReadonly();
  selectedTipoComida = this._selectedTipoComida.asReadonly();
  sortOrder = this._sortOrder.asReadonly();

  // 3. Variables Derivadas (Computed)
  tiposBebidaDisponibles = computed(() => {
    const bebidas = this._platos().filter(plato => {
      return (plato.categoria?.toLowerCase() || '').includes('bebida') || 
             (plato.tipo?.toLowerCase() || '').includes('bebida') || 
             !!plato.bebida;
    });
    const tiposMap = new Map<string, number>();
    bebidas.forEach(b => {
      let tipoFinal = 'Otros';
      const tipo = b.tipo?.trim();
      if (tipo && tipo.toLowerCase() !== 'bebida' && tipo.toLowerCase() !== 'bebidas') {
        tipoFinal = tipo;
      } else {
        const nombreLower = b.nombre.toLowerCase();
        if (nombreLower.includes('cerveza')) tipoFinal = 'Cerveza';
        else if (nombreLower.includes('vino')) tipoFinal = 'Vino';
        else if (nombreLower.includes('agua')) tipoFinal = 'Agua';
        else if (nombreLower.includes('jugo') || nombreLower.includes('exprimido')) tipoFinal = 'Jugo';
        else if (nombreLower.includes('coca-cola') || nombreLower.includes('sprite') || nombreLower.includes('fanta') || nombreLower.includes('pepsi') || nombreLower.includes('gaseosa')) tipoFinal = 'Gaseosa';
      }
      tiposMap.set(tipoFinal, (tiposMap.get(tipoFinal) || 0) + 1);
    });
    return Array.from(tiposMap.entries())
      .map(([tipo, count]) => ({ tipo, count }))
      .sort((a, b) => a.tipo.localeCompare(b.tipo));
  });

  totalBebidasCount = computed(() => {
    return this._platos().filter(plato => {
      return (plato.categoria?.toLowerCase() || '').includes('bebida') || 
             (plato.tipo?.toLowerCase() || '').includes('bebida') || 
             !!plato.bebida;
    }).length;
  });

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

  tiposComidaDisponibles = computed(() => {
    const comidas = this._platos().filter(plato => {
      const isBebida = (plato.categoria?.toLowerCase() || '').includes('bebida') || 
                       (plato.tipo?.toLowerCase() || '').includes('bebida') || 
                       !!plato.bebida;
      return !isBebida;
    });
    
    const tiposMap = new Map<string, number>();
    comidas.forEach(c => {
      let tipoFinal = c.categoria?.trim() || c.tipo?.trim() || 'Otros';
      if (tipoFinal.toLowerCase() === 'plato principal') tipoFinal = 'Principal';
      tiposMap.set(tipoFinal, (tiposMap.get(tipoFinal) || 0) + 1);
    });
    
    return Array.from(tiposMap.entries())
      .map(([tipo, count]) => ({ tipo, count }))
      .sort((a, b) => a.tipo.localeCompare(b.tipo));
  });

  totalComidasCount = computed(() => {
    return this._platos().filter(plato => {
      const isBebida = (plato.categoria?.toLowerCase() || '').includes('bebida') || 
                       (plato.tipo?.toLowerCase() || '').includes('bebida') || 
                       !!plato.bebida;
      return !isBebida;
    }).length;
  });

  platosComidas = computed(() => {
    const selectedTipo = this._selectedTipoComida();
    const normal = this.filteredPlatos().filter(plato => {
      const isBebida = (plato.categoria?.toLowerCase() || '').includes('bebida') || 
                       (plato.tipo?.toLowerCase() || '').includes('bebida') || 
                       !!plato.bebida;
      if (isBebida || (plato.recomendado && plato.visible)) return false;
      
      if (selectedTipo) {
        let tipoFinal = plato.categoria?.trim() || plato.tipo?.trim() || 'Otros';
        if (tipoFinal.toLowerCase() === 'plato principal') tipoFinal = 'Principal';
        return tipoFinal === selectedTipo;
      }
      return true;
    });
    return [...normal].sort((a, b) => {
      const currentSort = this._sortOrder();
      
      if (currentSort === 'ventas-desc') {
        return (b.ventas ?? 0) - (a.ventas ?? 0);
      } else if (currentSort === 'ventas-asc') {
        return (a.ventas ?? 0) - (b.ventas ?? 0);
      }
      
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

  platosBebidas = computed(() => {
    const selectedTipo = this._selectedTipoBebida();
    const normal = this.filteredPlatos().filter(plato => {
      const isBebida = (plato.categoria?.toLowerCase() || '').includes('bebida') || 
                       (plato.tipo?.toLowerCase() || '').includes('bebida') || 
                       !!plato.bebida;
      if (!isBebida || (plato.recomendado && plato.visible)) return false;
      
      if (selectedTipo) {
        let tipoPlato = plato.tipo?.trim();
        if (!tipoPlato || tipoPlato.toLowerCase() === 'bebida' || tipoPlato.toLowerCase() === 'bebidas') {
          const nombreLower = plato.nombre.toLowerCase();
          if (nombreLower.includes('cerveza')) tipoPlato = 'Cerveza';
          else if (nombreLower.includes('vino')) tipoPlato = 'Vino';
          else if (nombreLower.includes('agua')) tipoPlato = 'Agua';
          else if (nombreLower.includes('jugo') || nombreLower.includes('exprimido')) tipoPlato = 'Jugo';
          else if (nombreLower.includes('coca-cola') || nombreLower.includes('sprite') || nombreLower.includes('fanta') || nombreLower.includes('pepsi') || nombreLower.includes('gaseosa')) tipoPlato = 'Gaseosa';
          else tipoPlato = 'Otros';
        }
        return tipoPlato === selectedTipo || plato.categoria === selectedTipo;
      }
      return true;
    });
    return [...normal].sort((a, b) => {
      const currentSort = this._sortOrder();
      
      if (currentSort === 'ventas-desc') {
        return (b.ventas ?? 0) - (a.ventas ?? 0);
      } else if (currentSort === 'ventas-asc') {
        return (a.ventas ?? 0) - (b.ventas ?? 0);
      }
      
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
  setTipoBebida(tipo: string | null): void {
    this._selectedTipoBebida.set(tipo);
  }

  setTipoComida(tipo: string | null): void {
    this._selectedTipoComida.set(tipo);
  }

  setSortOrder(order: 'default' | 'ventas-desc' | 'ventas-asc'): void {
    this._sortOrder.set(order);
  }

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
