import { Mesa } from '../../domain/mesa';

export interface MesaOcuparResponse {
  mesa: Mesa;
  idComandaGenerada: number;
}
