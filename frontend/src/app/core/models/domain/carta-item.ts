export interface CartaItem {
  articuloId: number;
  nombre: string;
  urlImagen: string | null;
  precioVentaFinal: number;
  precio: number;
  costo: number;
  visibleEnCarta: boolean;
  tipoArticulo: 'Plato' | 'Bebida';
  categoria: string;

  tiempoPreparacionBase?: number;
  tiempoPreparacionEstimado?: number;

  restricciones: string[];


  tipoPlato?: string;
  categoriaPlato?: string;
  categoriaBebida?: string;
}