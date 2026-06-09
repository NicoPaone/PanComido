import { FormsModule } from '@angular/forms';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { RecetaIngrediente } from '../../../../../core/models/domain/plato';
import { IngredienteDisponibleDto } from '../../../services/plato.api';
import { Component, output, signal, computed, input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { UnidadMedida } from '../../../../../core/models/domain/unidad-medida';

interface RecipeQuantityPreset {
  label: string;
  value: number;
}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-detalle-receta',
  standalone: true,
  imports: [FormsModule, Buscador],
  templateUrl: './detalle-receta.html',
  styleUrl: './detalle-receta.css'
})
export class DetalleRecetaComponent implements OnInit {
  recetaCambiada = output<RecetaIngrediente[]>();
  ingredientesIniciales = input<RecetaIngrediente[]>([]);
  ingredientesDisponibles = input<IngredienteDisponibleDto[]>([]);

  busqueda = signal<string>('');
  ingredientesSeleccionados = signal<RecetaIngrediente[]>([]);


  sugerencias = computed(() => {
    const query = this.busqueda().toLowerCase().trim();
    if (!query) return [];

    return this.ingredientesDisponibles().filter(ing =>
      ing.nombre.toLowerCase().includes(query) &&
      !this.ingredientesSeleccionados().some(selected => selected.id === ing.id)
    );
  });

  onSearchChanged(value: string) {
    this.busqueda.set(value);
  }

  agregarIngrediente(ingrediente: IngredienteDisponibleDto) {
    const nuevo: RecetaIngrediente = {
      id: ingrediente.id,
      nombre: ingrediente.nombre,
      cantidad: 1,
      unidadMedida: ingrediente.unidadMedida,
      costoUnitario: ingrediente.costoUnitario
    };

    this.ingredientesSeleccionados.update(items => [...items, nuevo]);
    this.busqueda.set('');
    this.notificarCambio();
  }

  eliminarIngrediente(id: string | number) {
    this.ingredientesSeleccionados.update(items => items.filter(item => item.id !== id));
    this.notificarCambio();
  }

  onCantidadCambiada(item: RecetaIngrediente) {
    if (item.cantidad === null || item.cantidad === undefined || item.cantidad < 0.01) {
      item.cantidad = 0.01;
    }
    this.notificarCambio();
  }

  presetsCantidad(item: RecetaIngrediente): RecipeQuantityPreset[] {
    const unidad = this.unidadNormalizada(item.unidadMedida);

    if (['KG', 'KILO', 'KILOS'].includes(unidad)) {
      return [
        { label: '25 g', value: 0.025 },
        { label: '50 g', value: 0.05 },
        { label: '100 g', value: 0.1 },
        { label: '250 g', value: 0.25 },
        { label: '500 g', value: 0.5 }
      ];
    }

    if (['G', 'GR', 'GRAMO', 'GRAMOS'].includes(unidad)) {
      return [
        { label: '10 g', value: 10 },
        { label: '25 g', value: 25 },
        { label: '50 g', value: 50 },
        { label: '100 g', value: 100 },
        { label: '250 g', value: 250 }
      ];
    }

    if (['L', 'LT', 'LITRO', 'LITROS'].includes(unidad)) {
      return [
        { label: '25 ml', value: 0.025 },
        { label: '50 ml', value: 0.05 },
        { label: '100 ml', value: 0.1 },
        { label: '250 ml', value: 0.25 },
        { label: '500 ml', value: 0.5 }
      ];
    }

    if (['ML', 'MILILITRO', 'MILILITROS'].includes(unidad)) {
      return [
        { label: '10 ml', value: 10 },
        { label: '25 ml', value: 25 },
        { label: '50 ml', value: 50 },
        { label: '100 ml', value: 100 },
        { label: '250 ml', value: 250 }
      ];
    }

    if (['UN', 'U', 'UNIDAD', 'UNIDADES', 'PORCION', 'PORCIONES'].includes(unidad)) {
      return [
        { label: '+1', value: 1 },
        { label: '+2', value: 2 },
        { label: '+3', value: 3 }
      ];
    }

    return [];
  }

  sumarCantidadPreset(item: RecetaIngrediente, preset: RecipeQuantityPreset): void {
    const actual = Number(item.cantidad) || 0;
    item.cantidad = this.redondearCantidad(actual + preset.value);
    this.onCantidadCambiada(item);
  }

  limpiarCantidad(item: RecetaIngrediente): void {
    item.cantidad = 0.01;
    this.notificarCambio();
  }

  equivalenciaCantidad(item: RecetaIngrediente): string | null {
    const cantidad = Number(item.cantidad);
    if (!Number.isFinite(cantidad) || cantidad <= 0) return null;

    const unidad = this.unidadNormalizada(item.unidadMedida);
    if (['KG', 'KILO', 'KILOS'].includes(unidad)) {
      return `Equivale a ${this.formatearEquivalencia(cantidad, 'kg', 'g', 1000)}`;
    }

    if (['L', 'LT', 'LITRO', 'LITROS'].includes(unidad)) {
      return `Equivale a ${this.formatearEquivalencia(cantidad, 'l', 'ml', 1000)}`;
    }

    if (['G', 'GR', 'GRAMO', 'GRAMOS'].includes(unidad) && cantidad >= 1000) {
      return `Equivale a ${this.formatearEquivalencia(cantidad / 1000, 'kg', 'g', 1000)}`;
    }

    if (['ML', 'MILILITRO', 'MILILITROS'].includes(unidad) && cantidad >= 1000) {
      return `Equivale a ${this.formatearEquivalencia(cantidad / 1000, 'l', 'ml', 1000)}`;
    }

    return null;
  }

  cantidadPaso(item: RecetaIngrediente): number {
    const unidad = this.unidadNormalizada(item.unidadMedida);
    if (['UN', 'U', 'UNIDAD', 'UNIDADES', 'PORCION', 'PORCIONES'].includes(unidad)) return 1;
    if (['G', 'GR', 'GRAMO', 'GRAMOS', 'ML', 'MILILITRO', 'MILILITROS'].includes(unidad)) return 1;
    return 0.01;
  }

  private notificarCambio() {
    this.recetaCambiada.emit(this.ingredientesSeleccionados());
  }

  ngOnInit() {
    const iniciales = this.ingredientesIniciales();
    if (iniciales.length > 0) {
      this.ingredientesSeleccionados.set(iniciales);
      this.notificarCambio();
    }
  }

  private unidadNormalizada(unidad: string | UnidadMedida): string {
    const nombre = typeof unidad === 'string' ? unidad : unidad.nombre;
    return nombre.trim().toUpperCase();
  }

  private redondearCantidad(cantidad: number): number {
    return Math.round(cantidad * 1000) / 1000;
  }

  private formatearEquivalencia(cantidad: number, unidadMayor: string, unidadMenor: string, factor: number): string {
    const enteros = Math.trunc(cantidad);
    const menores = Math.round((cantidad - enteros) * factor);

    if (enteros > 0 && menores > 0) return `${enteros} ${unidadMayor} ${menores} ${unidadMenor}`;
    if (enteros > 0) return `${enteros} ${unidadMayor}`;
    return `${Math.round(cantidad * factor)} ${unidadMenor}`;
  }
}
