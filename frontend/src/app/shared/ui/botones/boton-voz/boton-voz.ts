import { Component, input, output , ChangeDetectionStrategy} from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-boton-voz',
  imports: [],
  templateUrl: './boton-voz.html',
  styleUrl: './boton-voz.css',
})
export class BotonVoz {

  enEscucha = input.required<boolean>();
  toggle = output<void>();
}
