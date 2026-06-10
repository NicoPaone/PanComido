import { Comanda, EstadoComanda, EstadoComandaId } from '../../../core/models/domain/comanda';

const ESTADO_REVERSE_MAP: Record<number, EstadoComanda> = {
  [EstadoComandaId.Nueva]: 'Nueva',
  [EstadoComandaId.EnPreparacion]: 'EnPreparacion',
  [EstadoComandaId.EnEspera]: 'EnEspera',
  [EstadoComandaId.Finalizada]: 'Finalizada',
};

// normaliza el estado que puede llegar como string o numero desde el backend
export function normalizarEstadoComanda(estado: string | number): EstadoComanda {
  if (typeof estado === 'number') {
    return ESTADO_REVERSE_MAP[estado] ?? 'Nueva';
  }
  if (typeof estado === 'string' && estado in ESTADO_REVERSE_MAP) {
    return ESTADO_REVERSE_MAP[estado as unknown as number] ?? 'Nueva';
  }
  return (estado as EstadoComanda) ?? 'Nueva';
}
