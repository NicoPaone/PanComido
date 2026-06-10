export interface CartaItem {
  articuloId: number;
  nombre: string;
  urlImagen: string | null;
  precioVentaFinal: number;
  costo: number;
  visibleEnCarta: boolean;
  tipoArticulo: 'Plato' | 'Bebida';
  categoria: string;

  tiempoPreparacionBase?: number;
}