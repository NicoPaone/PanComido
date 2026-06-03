import { computed, inject, Injectable, signal } from '@angular/core';
import { CartaItem } from '../../../../core/models/carta-item';
import { CartaService } from './carta-service';

@Injectable({
  providedIn: 'root',
})
export class CartaState {

  private api = inject(CartaService);
  private _items = signal<CartaItem[]>([]);
  private _cargando = signal(false);

  items = this._items.asReadonly();
  cargando = this._cargando.asReadonly();

  // Filtros
  busqueda = signal('');
  tiposSeleccionados = signal<string[]>([]);
  ordenarPor = signal('');

  // Computed: platos
  platos = computed(() =>
    this._items().filter(i => i.tipoArticulo === 'Plato')
  );

  // Computed: bebidas
  bebidas = computed(() =>
    this._items().filter(i => i.tipoArticulo === 'Bebida')
  );

  // Items filtrados (para la vista)
  itemsFiltrados = computed(() => {
    let resultado = this._items();

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
    this._cargando.set(true);
    this.api.obtenerCarta().subscribe({
      next: (data) => {
        this._items.set(data);
        this._cargando.set(false);
      },
      error: () => this._cargando.set(false)
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
