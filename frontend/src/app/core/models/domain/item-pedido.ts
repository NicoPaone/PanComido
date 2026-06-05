import { CartaItem } from './carta-item';

export interface ItemPedido {
  plato: CartaItem;
  cantidad: number;
  observacionesIngredientes?: string;
  observacionesGenerales?: string;
}
