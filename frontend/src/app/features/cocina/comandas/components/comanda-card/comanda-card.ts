import { Component, computed, input, output } from '@angular/core';
import { Comanda, EstadoComandaId } from '../../../../../core/models/comanda/comanda';
import { KdsContadorTiempo } from '../../../../../shared/ui/kds-contador-tiempo/kds-contador-tiempo';

@Component({
  selector: 'app-comanda-card',
  imports: [KdsContadorTiempo],
  templateUrl: './comanda-card.html',
  styleUrl: './comanda-card.css',
})
export class ComandaCard {


  comanda = input.required<Comanda>();
  accion = output<{ mesaId: number; estadoId: number }>();
readonly headerClass = computed(() => {
  const map: Record<string, string> = {
    'Nueva': 'bg-danger',
    'EnPreparacion': 'bg-success',
    'EnEspera': 'bg-warning',
  };
  return map[this.comanda().estado] ?? 'bg-gray';
});
  readonly siguienteEstado = computed<EstadoComandaId | null>(() => {
    const estado = this.comanda().estado;
    if (estado === 'Nueva') return EstadoComandaId.EnPreparacion;
    if (estado === 'EnPreparacion') return EstadoComandaId.EnEspera;
    if (estado === 'EnEspera') return EstadoComandaId.EnPreparacion;
    return null;
  });

 readonly textoBoton = computed(() => {
  const estado = this.comanda().estado;
  if (estado === 'Nueva') return 'ACEPTAR COMANDA';
  if (estado === 'EnPreparacion' || estado === 'EnEspera') return 'LLAMAR MOZO';
  return '';
});
  readonly puedeActuar = computed(() => this.siguienteEstado() !== null);

  onAccion(): void {
    const estado = this.siguienteEstado();
    if (estado === null) return;

    this.accion.emit({
      mesaId: this.comanda().mesaId,
      estadoId: estado,
    });
  }
}
