import { computed, inject, Injectable, signal } from '@angular/core';
import { CartaItem } from '../../../../core/models/domain/carta-item';
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
  categoriasSeleccionadas = signal<string[]>([]);
restriccionesSeleccionadas = signal<string[]>([]);


  

toggleCategoria(categoria: string): void {
  this.categoriasSeleccionadas.update(actual => {
    if (actual.includes(categoria)) {
      return actual.filter(c => c !== categoria);
    }

    return [...actual, categoria];
  });
}

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
    const categorias = this.categoriasSeleccionadas();

// if (categorias.length > 0) {
//   resultado = resultado.filter(item =>
//     item.tipoArticulo === 'Plato' &&
//     categorias.includes(item.categoria)
//   );
// }

const restricciones = this.restriccionesSeleccionadas();

if (restricciones.length > 0) {
  resultado = resultado.filter(item =>
    item.restricciones?.some(r =>
      restricciones.includes(r)
    )
  );
}

if (categorias.length > 0) {
  resultado = resultado.filter(item =>
    categorias.includes(item.categoria)
  );
}

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

  toggleRestriccion(restriccion: string): void {
  this.restriccionesSeleccionadas.update(actual => {
    if (actual.includes(restriccion)) {
      return actual.filter(r => r !== restriccion);
    }
    return [...actual, restriccion];
  });
}

  // limpiarFiltros(): void {
  //   this.busqueda.set('');
  //   this.tiposSeleccionados.set([]);
  //   this.ordenarPor.set('');
  // }

  limpiarFiltros(): void {
  this.busqueda.set('');
  this.tiposSeleccionados.set([]);
  this.categoriasSeleccionadas.set([]);
  this.ordenarPor.set('');
  this.restriccionesSeleccionadas.set([]);
}

  // tieneFiltrosActivos = computed(() =>
  //   this.tiposSeleccionados().length > 0
  // );
tieneFiltrosActivos = computed(() =>
  this.tiposSeleccionados().length > 0 ||
  this.categoriasSeleccionadas().length > 0 ||
  this.restriccionesSeleccionadas().length > 0 ||
  this.busqueda().trim() !== '' ||
  this.ordenarPor() !== ''
);

  cantidadFiltrosActivos = computed(() =>
    this.tiposSeleccionados().length +
    this.categoriasSeleccionadas().length +
    this.restriccionesSeleccionadas().length
  );

  
}

