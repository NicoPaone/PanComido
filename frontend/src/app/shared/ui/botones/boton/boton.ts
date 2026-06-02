import { CommonModule } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';

export type BotonVariante = 'primary' | 'secondary' | 'danger' | 'outline' | 'tab' | 'teal' | 'warning' | 'success';
export type BotonTamanio = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-boton',
  imports: [CommonModule],
  templateUrl: './boton.html',
  styleUrl: './boton.css',
})
export class Boton {
  @Input() variant: 'teal' | 'orange' | 'edit' | 'add' | 'delete' = 'teal';
  @Input() customClass: string = '';
}
