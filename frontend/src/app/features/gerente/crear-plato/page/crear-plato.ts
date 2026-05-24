import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent, RecetaIngrediente } from '../components/detalle-receta/detalle-receta';

@Component({
  selector: 'app-crear-plato',
  standalone: true,
  imports: [CommonModule, FormsModule, Boton, ToggleComponent, DetalleRecetaComponent],
  templateUrl: './crear-plato.html',
  styleUrl: './crear-plato.css'
})
export class CrearPlatoComponent {
  private router = inject(Router);

  nombre = signal<string>('Provolone fundido con morrones y tomates asados');
  costo = signal<number>(2100);
  precioVenta = signal<number>(4800);
  tiempoPreparacion = signal<number>(10);
  tipoPlato = signal<string>('Entrada');
  descripcion = signal<string>(
    'Queso provolone fundido al horno sobre base de morrones y tomates asados, condimentado con orégano, ají molido y aceite de oliva. Se sirve en cazuela individual bien caliente.'
  );
  visible = signal<boolean>(true);
  imagen = signal<string>('https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=300&h=250');

  vegano = signal<boolean>(false);
  vegetariano = signal<boolean>(true);
  celiaco = signal<boolean>(false);

  receta = signal<RecetaIngrediente[]>([]);

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
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  cancelar() {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
