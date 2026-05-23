import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Boton } from '../boton/boton';

@Component({
  selector: 'app-boton-categorias',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './boton-categorias.html'
})
export class BotonCategoriasComponent {
  isOpen = false;
  categorias = ['Todas', 'Entradas', 'Platos Principales', 'Postres', 'Bebidas'];

  toggle() {
    this.isOpen = !this.isOpen;
  }
}
