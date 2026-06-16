import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Plato, RecetaIngrediente } from '../../../../../core/models/domain/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { calcularCostoReceta } from '../../../services/plato-cost';
import { Insumo } from '../../../../../core/models/domain/insumo';
import { PlatoApiService } from '../../../services/plato.api';
import { ArsCurrencyPipe } from '../../../../../shared/pipes/ars-currency.pipe';
import { PriceNoteComponent } from '../../../../../shared/ui/price-note/price-note';

@Component({
  selector: 'app-modal-editar-plato',
  standalone: true,
  imports: [FormsModule, Boton, ToggleComponent, Buscador, ArsCurrencyPipe, PriceNoteComponent],
  templateUrl: './modal-editar-plato.html',
  styleUrls: ['./modal-editar-plato.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ModalEditarPlatoComponent {
  private api = inject(PlatoApiService);
  private destroyRef = inject(DestroyRef);

  plato = input.required<Plato>();
  save = output<Partial<Plato>>();
  close = output<void>();

  nombre = signal('');
  precioVenta = signal<number | null>(null);
  imagen = signal('');
  visible = signal(true);

  receta = signal<RecetaIngrediente[]>([]);
  busqueda = signal<string>('');
  insumos = signal<Insumo[]>([]);

  ingredientesBase = computed(() => this.receta().filter(i => !i.opcional));
  ingredientesOpcionales = computed(() => this.receta().filter(i => i.opcional));

  costo = computed(() => {
    return calcularCostoReceta(this.receta());
  });

  precioEsMenorQueCosto = computed(() => {
    const venta = this.precioVenta() ?? 0;
    const costoVal = this.costo() ?? 0;
    return venta > 0 && costoVal > 0 && venta <= costoVal;
  });

  sugerencias = computed(() => {
    const query = this.busqueda().toLowerCase().trim();
    if (!query) return [];
    
    return this.insumos().filter(prod =>
      prod.nombre.toLowerCase().includes(query) &&
      !this.receta().some(selected => selected.id === prod.id)
    );
  });

  constructor() {
    this.cargarInsumos();

    effect(() => {
      const p = this.plato();
      if (p) {
        this.nombre.set(p.nombre);
        this.precioVenta.set(p.precioVenta);
        this.imagen.set(p.imagen);
        this.visible.set(p.visible);
        const receta = p.receta ? JSON.parse(JSON.stringify(p.receta)) as RecetaIngrediente[] : [];
        this.receta.set(receta.map(ingrediente => ({
          ...ingrediente,
          unidadMedida: this.normalizarUnidadMedida(ingrediente.unidadMedida)
        })));
      }
    });
  }

  onToggleVisible() {
    this.visible.update(v => !v);
  }

  onSearchChanged(value: string) {
    this.busqueda.set(value);
  }

  cargarInsumos(): void {
    this.api.getInsumos()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: insumos => this.insumos.set(insumos),
        error: () => this.insumos.set([])
      });
  }

  agregarIngrediente(producto: Insumo) {
    const nuevo: RecetaIngrediente = {
      id: producto.id,
      nombre: producto.nombre,
      cantidad: 1,
      unidadMedida: this.normalizarUnidadMedida(producto.unidadMedida),
      costoUnitario: producto.precioVentaFinal ?? 0,
      opcional: false
    };

    this.receta.update(items => [...items, nuevo]);
    this.busqueda.set('');
  }

  eliminarIngrediente(id: string | number) {
    this.receta.update(items => items.filter(item => item.id !== id));
  }

  toggleOpcional(id: string | number) {
    this.receta.update(items => items.map(item => {
      if (item.id === id) {
        return { ...item, opcional: !item.opcional };
      }
      return item;
    }));
  }

  onCantidadCambiada(ing: RecetaIngrediente) {
    if (ing.cantidad === null || ing.cantidad === undefined || ing.cantidad < 0.01) {
      ing.cantidad = 0.01;
    }
  }

  onSave() {
    if (!this.nombre().trim() || this.precioVenta() === null || this.precioVenta()! <= 0 || this.costo() === null || this.costo()! <= 0) {
      return;
    }
    this.save.emit({
      nombre: this.nombre(),
      precioVenta: this.precioVenta()!,
      costo: this.costo()!,
      imagen: this.imagen(),
      visible: this.visible(),
      receta: this.receta()
    });
  }

  onClose() {
    this.close.emit();
  }

  private normalizarUnidadMedida(unidad: RecetaIngrediente['unidadMedida'] | Insumo['unidadMedida']): string {
    const valor = typeof unidad === 'string' ? unidad : unidad?.nombre ?? '';
    const normalizado = valor.trim().toLowerCase();

    if (['kg', 'kilo', 'kilos', 'kilogramo', 'kilogramos'].includes(normalizado)) return 'KG';
    if (['g', 'gr', 'gramo', 'gramos'].includes(normalizado)) return 'GR';
    if (['l', 'lt', 'lts', 'litro', 'litros'].includes(normalizado)) return 'L';
    if (['ml', 'mililitro', 'mililitros'].includes(normalizado)) return 'ML';
    if (['un', 'u', 'unidad', 'unidades'].includes(normalizado)) return 'UN';

    return valor.toUpperCase();
  }
}
