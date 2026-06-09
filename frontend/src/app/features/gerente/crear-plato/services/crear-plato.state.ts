import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PlatoApiService, ItemDesplegableDto, IngredienteDisponibleDto } from '../../services/plato.api';
import { CrearPlatoRequestDto } from '../../../../core/models/dtos/requests/crear-plato.request';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { calcularCostoReceta } from '../../services/plato-cost';

/** Mapeo de nombre de tipo/categoría a ID para el backend */
const TIPO_PLATO_MAP: Record<string, number> = {
  'Entrada': 1,
  'Plato Principal': 2,
  'Postre': 3,
  'Bebida': 4,
};

const CATEGORIA_PLATO_MAP: Record<string, number> = {
  'Entradas': 1,
  'Principales': 2,
  'Postres': 3,
  'Bebidas': 4,
};

@Injectable({ providedIn: 'root' })
export class CrearPlatoState {
  private api = inject(PlatoApiService);
  private destroyRef = inject(DestroyRef);

  visible = signal<boolean>(true);
  imagenSelected = signal<string>('');
  
  // Opciones desde el backend
  tiposPlato = signal<ItemDesplegableDto[]>([]);
  categoriasPlato = signal<ItemDesplegableDto[]>([]);
  restricciones = signal<ItemDesplegableDto[]>([]);
  ingredientesDisponibles = signal<IngredienteDisponibleDto[]>([]);
  
  restriccionesSeleccionadas = signal<number[]>([]);
  vegano = computed(() => this.restriccionesSeleccionadas().includes(1));
  vegetariano = computed(() => this.restriccionesSeleccionadas().includes(2));
  celiaco = computed(() => this.restriccionesSeleccionadas().includes(3));
  receta = signal<RecetaIngrediente[]>([]);
  mostrarExito = signal<boolean>(false);
  mostrarSelectorImagen = signal<boolean>(false);

  readonly #loading = signal<boolean>(false);
  loading = this.#loading.asReadonly();

  costoSugerido = computed(() => {
    return calcularCostoReceta(this.receta());
  });

  cargarDatosFormulario(): void {
    this.#loading.set(true);
    this.api.getDatosFormulario()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => {
          this.tiposPlato.set(res.tiposPlato);
          this.categoriasPlato.set(res.categoriasPlato);
          this.restricciones.set(res.restricciones);
          this.ingredientesDisponibles.set(res.ingredientes);
          this.#loading.set(false);
        },
        error: () => {
          this.#loading.set(false);
        }
      });
  }

  toggleRestriccion(id: number): void {
    const current = this.restriccionesSeleccionadas();
    if (current.includes(id)) {
      this.restriccionesSeleccionadas.set(current.filter(x => x !== id));
    } else {
      this.restriccionesSeleccionadas.set([...current, id]);
    }
  }

  toggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco'): void {
    const ids = { vegano: 1, vegetariano: 2, celiaco: 3 } as const;
    this.toggleRestriccion(ids[tag]);
  }

  toggleVisible(): void {
    this.visible.update(v => !v);
  }

  updateReceta(ingredientes: RecetaIngrediente[]): void {
    this.receta.set(ingredientes);
  }

  abrirSelectorImagen(): void {
    this.mostrarSelectorImagen.set(true);
  }

  cerrarSelectorImagen(): void {
    this.mostrarSelectorImagen.set(false);
  }

  seleccionarImagen(url: string): void {
    this.imagenSelected.set(url);
    this.mostrarSelectorImagen.set(false);
  }

  setMostrarExito(val: boolean): void {
    this.mostrarExito.set(val);
  }

  guardarPlato(platoData: { nombre: string; costo: number; precioVenta: number; tiempoPreparacion: number; tipoPlato: string; descripcion: string; }, callback: () => void): void {
    this.#loading.set(true);

    const request: CrearPlatoRequestDto = {
      nombre: platoData.nombre,
      descripcion: platoData.descripcion,
      precioVentaFinal: platoData.precioVenta,
      tiempoPreparacionBase: platoData.tiempoPreparacion,
      tipoPlatoId: TIPO_PLATO_MAP[platoData.tipoPlato] ?? 2,
      categoriaPlatoId: CATEGORIA_PLATO_MAP[platoData.tipoPlato] ?? 2,
      urlImagen: this.imagenSelected() || '',
      restriccionesIds: this.restriccionesSeleccionadas(),
      ingredientes: this.receta().map(ing => ({
        insumoId: Number(ing.id),
        cantidad: ing.cantidad,
        opcional: false
      }))
    };

    this.api.crearPlato(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.#loading.set(false);
          this.mostrarExito.set(true);
          callback();
        },
        error: (err) => {
          console.error('Error al crear plato:', err?.error?.error || err?.message || err);
          this.#loading.set(false);
        }
      });
  }
}
