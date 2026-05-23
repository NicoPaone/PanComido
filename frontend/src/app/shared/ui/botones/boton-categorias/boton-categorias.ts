import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Boton } from '../boton/boton';

@Component({
  selector: 'app-boton-categorias',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './boton-categorias.html'
})
export class BotonCategoriasComponent {
  // Entrada: lista de categorías (configurable)
  categorias = input<string[]>(['Todas', 'Entradas', 'Platos Principales', 'Postres', 'Bebidas']);
  
  // Estado interno: si el dropdown está abierto
  isOpen = signal(false);
  
  // Salida: cuando se selecciona una categoría
  categoriaSeleccionada = output<string>();

  /**
   * Toggle del dropdown
   */
  toggle() {
    this.isOpen.set(!this.isOpen());
  }

  /**
   * Selecciona una categoría y cierra el dropdown
   */
  seleccionar(categoria: string) {
    this.categoriaSeleccionada.emit(categoria);
    this.isOpen.set(false);
  }
}
