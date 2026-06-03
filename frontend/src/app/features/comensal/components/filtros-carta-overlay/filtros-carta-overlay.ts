import { Component, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CartaState } from '../../ver-carta/service/carta-state';

interface CategoriaFiltro {
  id: 'platos' | 'bebidas' | 'restricciones';
  nombre: string;
  icono: string;
  subcategorias: SubcategoriaFiltro[];
  expandido: boolean;
}

interface SubcategoriaFiltro {
  valor: string;
  label: string;
}

@Component({
  selector: 'app-filtros-carta-overlay',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './filtros-carta-overlay.html',
  styleUrls: ['./filtros-carta-overlay.css']
})
export class FiltrosCartaOverlay {
  cartaState = inject(CartaState);
  cerrar = output<void>();

  expandidoPlatos = true;
  expandidoBebidas = false;
  expandidoRestricciones = false;

  // Mapeo de valores del backend a labels para el usuario
  subcategorias = {
    platos: [
      { valor: 'Entradas', label: 'Entradas' },
      { valor: 'Principales', label: 'Principales' },
      { valor: 'Guarniciones', label: 'Guarniciones' },
      { valor: 'Postres', label: 'Postres' },
      { valor: 'Promos', label: 'Promos' }
    ],
    bebidas: [
      { valor: 'Sin alcohol', label: 'Sin alcohol' },
      { valor: 'Con alcohol', label: 'Con alcohol' }
    ],
    restricciones: [
      { valor: 'Vegano', label: 'Vegano' },
      { valor: 'Celíaco', label: 'Celíaco' },
      { valor: 'Vegetariano', label: 'Vegetariano' }
    ]
  };

  toggleCategoria(categoria: 'platos' | 'bebidas' | 'restricciones'): void {
    switch(categoria) {
      case 'platos':
        this.expandidoPlatos = !this.expandidoPlatos;
        break;
      case 'bebidas':
        this.expandidoBebidas = !this.expandidoBebidas;
        break;
      case 'restricciones':
        this.expandidoRestricciones = !this.expandidoRestricciones;
        break;
    }
  }

  limpiarFiltros(): void {
    this.cartaState.limpiarFiltros();
  }
}
