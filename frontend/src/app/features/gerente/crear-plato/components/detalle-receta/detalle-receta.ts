import { FormsModule } from '@angular/forms';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { RecetaIngrediente } from '../../../../../core/models/domain/plato';
import { Insumo } from '../../../../../core/models/domain/insumo';
import { INSUMOS_MOCK } from '../../../../../infra/mocks/insumo.mock';
import { Component, output, signal, computed, input, OnInit , ChangeDetectionStrategy} from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-detalle-receta',
  standalone: true,
  imports: [FormsModule, Buscador],
  templateUrl: './detalle-receta.html',
  styleUrl: './detalle-receta.css'
})
export class DetalleRecetaComponent {
  recetaCambiada = output<RecetaIngrediente[]>();
ingredientesIniciales = input<RecetaIngrediente[]>([]);
  busqueda = signal<string>('');
  
  ingredientesSeleccionados = signal<RecetaIngrediente[]>([]);

  sugerencias = computed(() => {
    const query = this.busqueda().toLowerCase().trim();
    if (!query) return [];
    
    return INSUMOS_MOCK.filter(prod => 
      prod.nombre.toLowerCase().includes(query) &&
      !this.ingredientesSeleccionados().some(selected => selected.id === prod.id)
    );
  });

  onSearchChanged(value: string) {
    this.busqueda.set(value);
  }

  agregarIngrediente(producto: Insumo) {
    const nuevo: RecetaIngrediente = {
      id: producto.id,
      nombre: producto.nombre,
      cantidad: 1,
      unidadMedida: producto.unidadMedida
    };

    this.ingredientesSeleccionados.update(items => [...items, nuevo]);
    this.busqueda.set('');
    this.notificarCambio();
  }

  eliminarIngrediente(id: string|number) {
    this.ingredientesSeleccionados.update(items => items.filter(item => item.id !== id));
    this.notificarCambio();
  }

  onCantidadCambiada(item: RecetaIngrediente) {
    if (item.cantidad === null || item.cantidad === undefined || item.cantidad < 0.01) {
      item.cantidad = 0.01;
    }
    this.notificarCambio();
  }

  onUnidadCambiada() {
    this.notificarCambio();
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
}
