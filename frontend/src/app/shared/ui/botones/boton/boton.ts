import { CommonModule } from '@angular/common';
import { Component, computed, Input, input, output } from '@angular/core';

export type BotonVariante = 'primary' | 'secondary' | 'danger' | 'outline' | 'tab' | 'teal' | 'warning' | 'success';
export type BotonTamanio = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-boton',
  imports: [CommonModule],
  templateUrl: './boton.html',
  styleUrl: './boton.css',
})
export class Boton {
  //@Input() variant: 'teal' | 'orange' | 'edit' | 'add' | 'delete' = 'teal';
  
  label = input<string>();
  variante = input<BotonVariante>('primary');
  color = input<string>();
  tamanio = input<BotonTamanio>('md');
  type = input<'button' | 'submit' | 'reset'>('button');
  activo = input<boolean>(false);

  disabled = input<boolean>(false);

  clicked = output<MouseEvent>();

  
  clasesCalculadas = computed(() => {
    const baseClasses = `btn btn-${this.tamanio()} btn-${this.variante()}`;
    return this.activo() ? `${baseClasses} active` : baseClasses;
  });

  manejarClick(event: MouseEvent) {
    if (!this.disabled()) {
      this.clicked.emit(event);
    }
  }
}
