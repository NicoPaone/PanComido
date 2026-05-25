import { Component, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { PRODUCTOS_STOCK_MOCK, ProductoStockMock, UnidadMedida } from '../../../../../core/model/producto-stock-mock';
import { RecetaIngrediente } from '../../../../../core/models/plato';

@Component({
  selector: 'app-detalle-receta',
  standalone: true,
  imports: [CommonModule, FormsModule, Buscador],
  templateUrl: './detalle-receta.html',
  styleUrl: './detalle-receta.css'
})
export class DetalleRecetaComponent {
  recetaCambiada = output<RecetaIngrediente[]>();

  busqueda = signal<string>('');
  
  ingredientesSeleccionados = signal<RecetaIngrediente[]>([]);

  sugerencias = computed(() => {
    const query = this.busqueda().toLowerCase().trim();
    if (!query) return [];
    
    return PRODUCTOS_STOCK_MOCK.filter(prod => 
      prod.nombre.toLowerCase().includes(query) &&
      !this.ingredientesSeleccionados().some(selected => selected.id === prod.id)
    );
  });

  onSearchChanged(value: string) {
    this.busqueda.set(value);
  }

  agregarIngrediente(producto: ProductoStockMock) {
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

  eliminarIngrediente(id: string) {
    this.ingredientesSeleccionados.update(items => items.filter(item => item.id !== id));
    this.notificarCambio();
  }

  onCantidadCambiada() {
    this.notificarCambio();
  }

  onUnidadCambiada() {
    this.notificarCambio();
  }

  private notificarCambio() {
    this.recetaCambiada.emit(this.ingredientesSeleccionados());
  }
}
