import { Component, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Buscador } from '../../../../../shared/ui/buscador/buscador';
import { PRODUCTOS_STOCK_MOCK, ProductoStockMock, UnidadMedida } from '../../../../../core/model/producto-stock-mock';

export interface RecetaIngrediente {
  id: string;
  nombre: string;
  cantidad: number;
  unidadMedida: UnidadMedida;
}

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
  
  ingredientesSeleccionados = signal<RecetaIngrediente[]>([
    { id: '12', nombre: 'Aceite de oliva', cantidad: 10, unidadMedida: 'L' },
    { id: '13', nombre: 'Ají molido', cantidad: 5, unidadMedida: 'GR' },
    { id: '14', nombre: 'Morrón asado', cantidad: 200, unidadMedida: 'GR' },
    { id: '15', nombre: 'Orégano', cantidad: 20, unidadMedida: 'GR' },
    { id: '16', nombre: 'Provoleta', cantidad: 170, unidadMedida: 'GR' },
    { id: '17', nombre: 'Tomate asado', cantidad: 200, unidadMedida: 'GR' }
  ]);

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
