import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent } from '../components/detalle-receta/detalle-receta';
import { RecetaIngrediente } from '../../../../core/models/plato';

@Component({
  selector: 'app-crear-plato',
  standalone: true,
  imports: [CommonModule, FormsModule, Boton, ToggleComponent, DetalleRecetaComponent],
  templateUrl: './crear-plato.html',
  styleUrl: './crear-plato.css'
})
export class CrearPlatoComponent {
  private router = inject(Router);

  nombre = signal<string>('');
  costo = signal<number | null>(null);
  precioVenta = signal<number | null>(null);
  tiempoPreparacion = signal<number | null>(null);
  tipoPlato = signal<string>('');
  descripcion = signal<string>('');
  visible = signal<boolean>(true);
  imagen = signal<string>('');

  vegano = signal<boolean>(false);
  vegetariano = signal<boolean>(false);
  celiaco = signal<boolean>(false);

  receta = signal<RecetaIngrediente[]>([]);
  mostrarExito = signal<boolean>(false);

  toggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco') {
    if (tag === 'vegano') {
      this.vegano.update(v => !v);
    } else if (tag === 'vegetariano') {
      this.vegetariano.update(v => !v);
    } else if (tag === 'celiaco') {
      this.celiaco.update(v => !v);
    }
  }

  onToggleVisible() {
    this.visible.update(v => !v);
  }

  onRecetaCambiada(ingredientes: RecetaIngrediente[]) {
    this.receta.set(ingredientes);
  }

  guardar() {
    const platoFinal = {
      nombre: this.nombre(),
      costo: this.costo(),
      precioVenta: this.precioVenta(),
      tiempoPreparacion: this.tiempoPreparacion(),
      tipoPlato: this.tipoPlato(),
      descripcion: this.descripcion(),
      visible: this.visible(),
      imagen: this.imagen(),
      tags: {
        vegano: this.vegano(),
        vegetariano: this.vegetariano(),
        celiaco: this.celiaco()
      },
      receta: this.receta()
    };

    console.log('Guardando plato:', platoFinal);
    this.mostrarExito.set(true);
  }

  cerrarExito() {
    this.mostrarExito.set(false);
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  cancelar() {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
