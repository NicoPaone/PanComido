import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Mesa } from '../../../../core/models/mesa.model';

@Component({
  selector: 'app-mesa-item',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mesa-item.html',
  styleUrl: './mesa-item.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MesaItem {
  mesa = input.required<Mesa>();
  isEditorMode = input<boolean>(false); // Para saber si mostramos el input o el texto

  // Output para avisarle al padre si hicieron click
  clickMesa = output<number>();

  // Cálculos dinámicos
  ancho = computed(() => this.mesa().posicionXfin - this.mesa().posicionXInicio);
  alto = computed(() => this.mesa().posicionYFin - this.mesa().posicionYinicio);

  // Mapeo dinámico de clases CSS según el enum
  claseEstado = computed(() => `estado-${this.mesa().estadoMesa}`);
  claseForma = computed(() => `forma-${this.mesa().dimensionMesa.forma}`);

  cambioNumero = output<{id: number, numero: number}>();
  eliminar = output<number>();

  onInputBlur(event: Event) {
    const nuevoValor = parseInt((event.target as HTMLInputElement).value, 10);
    if (!isNaN(nuevoValor)) {
      this.cambioNumero.emit({ id: this.mesa().id, numero: nuevoValor });
    }
  }
}
