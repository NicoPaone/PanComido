import { Component, input, output } from '@angular/core';
import { TurnoLaboral } from '../../../../../core/models/domain/turno-laboral';

@Component({
  selector: 'app-turnos-laborales-list',
  imports: [],
  templateUrl: './turnos-laborales-list.html',
  styleUrl: './turnos-laborales-list.css',
})
export class TurnosLaboralesList {
  readonly turnos = input.required<TurnoLaboral[]>();
  readonly turnoChange = output<{
    id: number;
    cambios: Partial<Pick<TurnoLaboral, 'horarioInicio' | 'horarioFin'>>;
  }>();

  onCambio(
    id: number,
    campo: 'horarioInicio' | 'horarioFin',
    event: Event
  ): void {
    let value = (event.target as HTMLInputElement).value;
    // type="time" devuelve HH:mm, pero el backend espera HH:mm:ss
    if (value && value.length === 5) {
      value = `${value}:00`;
    }
    this.turnoChange.emit({ id, cambios: { [campo]: value } });
  }
}