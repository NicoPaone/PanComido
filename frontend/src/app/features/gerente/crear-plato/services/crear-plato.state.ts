import { Injectable, inject, signal, computed, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CrearPlatoApiService, ItemDesplegableDto, IngredienteDisponibleDto } from './crear-plato.api';
import { Plato, RecetaIngrediente } from '../../../../core/models/domain/plato';
import { calcularCostoReceta } from '../../services/plato.service';

@Injectable({ providedIn: 'root' })
export class CrearPlatoState {
  private api = inject(CrearPlatoApiService);
  private destroyRef = inject(DestroyRef);

  // Estado centralizado
  visible = signal<boolean>(true);
  imagenSelected = signal<string>('');
  
  // Opciones desde el backend
  tiposPlato = signal<ItemDesplegableDto[]>([]);
  categoriasPlato = signal<ItemDesplegableDto[]>([]);
  restricciones = signal<ItemDesplegableDto[]>([]);
  ingredientesDisponibles = signal<IngredienteDisponibleDto[]>([]);
  
  restriccionesSeleccionadas = signal<number[]>([]);
  receta = signal<RecetaIngrediente[]>([]);
  mostrarExito = signal<boolean>(false);
  mostrarSelectorImagen = signal<boolean>(false);

  private _loading = signal<boolean>(false);
  loading = this._loading.asReadonly();

  costoSugerido = computed(() => {
    return calcularCostoReceta(this.receta());
  });

  cargarDatosFormulario(): void {
    this._loading.set(true);
    this.api.getDatosFormulario()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (res) => {
          this.tiposPlato.set(res.tiposPlato);
          this.categoriasPlato.set(res.categoriasPlato);
          this.restricciones.set(res.restricciones);
          this.ingredientesDisponibles.set(res.ingredientes);
          this._loading.set(false);
        },
        error: () => {
          this._loading.set(false);
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

  guardarPlato(platoData: { nombre: string; costo: number; precioVenta: number; tiempoPreparacion: number; tipoPlatoId: number; categoriaPlatoId: number; descripcion: string; }, callback: () => void): void {
    this._loading.set(true);

    const request = {
      nombre: platoData.nombre,
      descripcion: platoData.descripcion,
      precioVentaFinal: platoData.precioVenta,
      tiempoPreparacionBase: platoData.tiempoPreparacion,
      tipoPlatoId: platoData.tipoPlatoId,
      categoriaPlatoId: platoData.categoriaPlatoId,
      urlImagen: this.imagenSelected() || '',
      restriccionesIds: this.restriccionesSeleccionadas(),
      ingredientes: this.receta().map(ing => ({
        insumoId: Number(ing.id),
        cantidad: ing.cantidad,
        opcional: false
      }))
    };

    this.api.crearPlato(request as any)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this._loading.set(false);
          this.mostrarExito.set(true);
          callback();
        },
        error: () => {
          this._loading.set(false);
        }
      });
  }
}
