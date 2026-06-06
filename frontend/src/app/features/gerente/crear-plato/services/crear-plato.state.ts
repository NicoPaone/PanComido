import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CrearPlatoApiService } from './crear-plato.api';
import { CrearPlatoRequestDto } from '../../../../core/models/dtos/requests/crear-plato.request';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { calcularCostoReceta } from '../../services/plato.service';

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
  private api = inject(CrearPlatoApiService);
  private destroyRef = inject(DestroyRef);

  visible = signal<boolean>(true);
  imagenSelected = signal<string>('');
  vegano = signal<boolean>(false);
  vegetariano = signal<boolean>(false);
  celiaco = signal<boolean>(false);
  receta = signal<RecetaIngrediente[]>([]);
  mostrarExito = signal<boolean>(false);
  mostrarSelectorImagen = signal<boolean>(false);

  private _loading = signal<boolean>(false);
  loading = this._loading.asReadonly();

  costoSugerido = computed(() => {
    return calcularCostoReceta(this.receta());
  });

  // Métodos de Negocio
  toggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco'): void {
    if (tag === 'vegano') {
      this.vegano.update(v => !v);
    } else if (tag === 'vegetariano') {
      this.vegetariano.update(v => !v);
    } else if (tag === 'celiaco') {
      this.celiaco.update(v => !v);
    }
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
    this._loading.set(true);

    const restriccionesIds: number[] = [];
    if (this.vegano()) restriccionesIds.push(1);
    if (this.vegetariano()) restriccionesIds.push(2);
    if (this.celiaco()) restriccionesIds.push(3);

    const request: CrearPlatoRequestDto = {
      nombre: platoData.nombre,
      descripcion: platoData.descripcion,
      precioVentaFinal: platoData.precioVenta,
      tiempoPreparacionBase: platoData.tiempoPreparacion,
      tipoPlatoId: TIPO_PLATO_MAP[platoData.tipoPlato] ?? 2,
      categoriaPlatoId: CATEGORIA_PLATO_MAP[platoData.tipoPlato] ?? 2,
      urlImagen: this.imagenSelected() || '',
      restriccionesIds,
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
          this._loading.set(false);
          this.mostrarExito.set(true);
          callback();
        },
        error: (err) => {
          console.error('Error al crear plato:', err?.error?.error || err?.message || err);
          this._loading.set(false);
        }
      });
  }
}
