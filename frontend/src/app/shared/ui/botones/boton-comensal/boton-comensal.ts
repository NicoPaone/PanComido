import { CommonModule } from '@angular/common';
import { Component, computed, Input, input, output } from '@angular/core';

export type BotonVariante = 'primary' | 'secondary' | 'danger' | 'outline' | 'tab' | 'teal' | 'warning' | 'success';
export type BotonTamanio = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-boton-comensal',
  imports: [CommonModule],
  templateUrl: './boton-comensal.html',
  styleUrl: './boton-comensal.css',
})
export class BotonComensal {
   label = input<string>();
   disabled = input<boolean>(false);

  backgroundColor = input<string>();
  textColor = input<string>();

  clicked = output<MouseEvent>();
}
