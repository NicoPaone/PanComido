import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { Mesa } from '../../../core/models/mesa.model';

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
  isEditorMode = input<boolean>(false);
  isSeleccionada = input<boolean>(false);
  enColision = input<boolean>(false);

  clickMesa = output<number>();
  accionMenu = output<'ocupar' | 'detalles' | 'deshabilitar' | 'cerrar' | 'abrir'>();

  ancho = computed(() => this.mesa().posicionXFin - this.mesa().posicionXInicio);
  alto = computed(() => this.mesa().posicionYFin - this.mesa().posicionYInicio);
  claseEstado = computed(() => `estado-${this.mesa().estadoMesa.toLocaleLowerCase()}`);
  claseForma = computed(() => `forma-${this.mesa().dimensionMesa.forma}`);
  abrirHaciaArriba = computed(() => this.mesa().posicionYInicio > 450);

  cambioNumero = output<{id: number, numero: number}>();
  eliminar = output<number>();

  manejarClickMesa(event: Event) {
    event.stopPropagation();
    this.clickMesa.emit(this.mesa().id);
  }

  onInputBlur(event: Event) {
    const nuevoValor = parseInt((event.target as HTMLInputElement).value, 10);
    if (!isNaN(nuevoValor)) {
      this.cambioNumero.emit({ id: this.mesa().id, numero: nuevoValor });
    }
  }
}
