export interface CartaItem {
 articuloId: number;
 id: number;
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

  esPlato: boolean;

  tipoPlato?: string;
  categoriaPlato?: string;
  categoriaBebida?: string;
}