import { computed, DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CartaItem } from '../../../../core/models/domain/carta-item';
import { CartaService } from './carta-service';

@Injectable({
  providedIn: 'root',
})
export class CartaState {

  private api = inject(CartaService);
  private destroyRef = inject(DestroyRef);
  readonly #items = signal<CartaItem[]>([]);
  readonly #cargando = signal(false);

  items = this.#items.asReadonly();
  cargando = this.#cargando.asReadonly();

  // Filtros
  busqueda = signal('');
  tiposSeleccionados = signal<string[]>([]);
  ordenarPor = signal('');

  // Computed: platos
  platos = computed(() =>
    this.#items().filter(i => i.tipoArticulo === 'Plato')
  );

  // Computed: bebidas
  bebidas = computed(() =>
    this.#items().filter(i => i.tipoArticulo === 'Bebida')
  );

  // Items filtrados (para la vista)
  itemsFiltrados = computed(() => {
    let resultado = this.#items();

    // Filtro por búsqueda (solo nombre, descripcion no existe)
    const busqueda = this.busqueda().toLowerCase();
    if (busqueda) {
      resultado = resultado.filter(i =>
        i.nombre.toLowerCase().includes(busqueda)
      );
    }

    // Filtro por tipo (Plato / Bebida)
    const tipos = this.tiposSeleccionados();
    if (tipos.length > 0) {
      resultado = resultado.filter(i =>
        tipos.includes(i.tipoArticulo)
      );
    }

    // Ordenamiento
    const orden = this.ordenarPor();
    switch (orden) {
      case 'precio-menor':
        resultado = [...resultado].sort((a, b) => a.precioVentaFinal - b.precioVentaFinal);
        break;
      case 'precio-mayor':
        resultado = [...resultado].sort((a, b) => b.precioVentaFinal - a.precioVentaFinal);
        break;
      case 'nombre':
        resultado = [...resultado].sort((a, b) =>
          a.nombre.localeCompare(b.nombre)
        );
        break;
    }

    return resultado;
  });

  cargarCarta(): void {
    this.#cargando.set(true);
    this.api.obtenerCarta().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => {
        this.#items.set(data);
        this.#cargando.set(false);
      },
      error: () => this.#cargando.set(false)
    });
  }

  setBusqueda(valor: string): void {
    this.busqueda.set(valor);
  }

  setOrdenar(valor: string): void {
    this.ordenarPor.set(valor);
  }

  toggleTipoPlato(tipo: string): void {
    this.tiposSeleccionados.update(tipos => {
      if (tipos.includes(tipo)) {
        return tipos.filter(t => t !== tipo);
      }
      return [...tipos, tipo];
    });
  }

  limpiarFiltros(): void {
    this.busqueda.set('');
    this.tiposSeleccionados.set([]);
    this.ordenarPor.set('');
  }

  tieneFiltrosActivos = computed(() =>
    this.tiposSeleccionados().length > 0
  );

  cantidadFiltrosActivos = computed(() =>
    this.tiposSeleccionados().length
  );
}
