import { ChangeDetectionStrategy, Component, DestroyRef, computed, effect, inject, input, output, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Plato, RecetaIngrediente } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { calcularCostoReceta } from '../../../../../core/services/plato.service';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { ModificarCartaApiService } from '../../services/modificar-carta.api';

@Component({
  selector: 'app-modal-editar-plato',
  standalone: true,
  imports: [FormsModule, Boton, ToggleComponent, Buscador],
  templateUrl: './modal-editar-plato.html',
  styleUrls: ['./modal-editar-plato.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ModalEditarPlatoComponent {
  private api = inject(ModificarCartaApiService);
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
        this.receta.set(p.receta ? JSON.parse(JSON.stringify(p.receta)) : []);
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
      unidadMedida: producto.unidadMedida
    };

    this.receta.update(items => [...items, nuevo]);
    this.busqueda.set('');
  }

  eliminarIngrediente(id: string | number) {
    this.receta.update(items => items.filter(item => item.id !== id));
  }

  onCantidadCambiada(ing: RecetaIngrediente) {
    if (ing.cantidad === null || ing.cantidad === undefined || ing.cantidad < 0.01) {
      ing.cantidad = 0.01;
    }
  }

  onUnidadCambiada() {
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
}
