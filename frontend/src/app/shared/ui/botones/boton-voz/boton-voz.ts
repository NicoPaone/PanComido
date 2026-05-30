import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-boton-voz',
  imports: [],
  templateUrl: './boton-voz.html',
  styleUrl: './boton-voz.css',
})
export class BotonVoz {

  enEscucha = input.required<boolean>();
  toggle = output<void>();
}
