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

  // Output para avisarle al padre si hicieron click
  clickMesa = output<number>();

  // Cálculos dinámicos
  ancho = computed(() => this.mesa().posicionXfin - this.mesa().posicionXInicio);
  alto = computed(() => this.mesa().posicionYFin - this.mesa().posicionYinicio);

  // Mapeo dinámico de clases CSS según el enum
  claseEstado = computed(() => `estado-${this.mesa().estadoMesa}`);
  claseForma = computed(() => `forma-${this.mesa().dimensionMesa.forma}`);
}
