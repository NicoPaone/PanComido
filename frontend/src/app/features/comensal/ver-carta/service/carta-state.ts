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
  bebidasSeleccionadas = signal<string[]>([]);
  restriccionesSeleccionadas = signal<string[]>([]);
  ordenarPor = signal('');

  // Computed: platos (esPlato = true)
  platos = computed(() =>
    this._items().filter(i => i.esPlato)
  );

  // Comandas: bebidas (esPlato = false)
  bebidas = computed(() =>
    this._items().filter(i => !i.esPlato)
  );

  // Destacados
  destacados = computed(() =>
    this._items().filter(i => i.esDestacado)
  );

  // Items filtrados (para la vista)
  itemsFiltrados = computed(() => {
    let resultado = this._items();

    // Filtro por búsqueda
    const busqueda = this.busqueda().toLowerCase();
    if (busqueda) {
      resultado = resultado.filter(i =>
        i.nombre.toLowerCase().includes(busqueda) ||
        i.descripcion.toLowerCase().includes(busqueda)
      );
    }

    // Filtro por tipo de plato
    const tipos = this.tiposSeleccionados();
    if (tipos.length > 0) {
      resultado = resultado.filter(i =>
        i.categoriaPlato && tipos.includes(i.categoriaPlato)
      );
    }

    // Filtro por bebida
    const bebidas = this.bebidasSeleccionadas();
    if (bebidas.length > 0) {
      resultado = resultado.filter(i =>
        i.categoriaBebida && bebidas.includes(i.categoriaBebida)
      );
    }

    // Filtro por restricciones
    const restricciones = this.restriccionesSeleccionadas();
    if (restricciones.length > 0) {
      resultado = resultado.filter(i =>
        restricciones.some(r => i.restricciones.includes(r))
      );
    }

    // Ordenamiento
    const orden = this.ordenarPor();
    switch (orden) {
      case 'precio-menor':
        resultado = [...resultado].sort((a, b) => a.precio - b.precio);
        break;
      case 'precio-mayor':
        resultado = [...resultado].sort((a, b) => b.precio - a.precio);
        break;
      case 'tiempo':
        resultado = [...resultado].sort((a, b) =>
          (a.tiempoPreparacionBase ?? 0) - (b.tiempoPreparacionBase ?? 0)
        );
        break;
      case 'nombre':
        resultado = [...resultado].sort((a, b) =>
          a.nombre.localeCompare(b.nombre)
        );
        break;
    }

    return resultado;
  });

  cargarCarta(restauranteId: number): void {
    this._cargando.set(true);
   
 this.api.obtenerCarta().subscribe({
  next: (data) => {
    console.log('DATOS:', data);
    this._items.set(data);
    this._cargando.set(false);
  },
  error: (err) => {
    console.error('ERROR API:', err);
    this._cargando.set(false);
  }
});
  }

  setBusqueda(valor: string): void {
    this.busqueda.set(valor);
  }

  setOrdenar(valor: string): void {
    this.ordenarPor.set(valor);
  }

  limpiarFiltros(): void {
    this.busqueda.set('');
    this.tiposSeleccionados.set([]);
    this.bebidasSeleccionadas.set([]);
    this.restriccionesSeleccionadas.set([]);
    this.ordenarPor.set('');
  }
}
