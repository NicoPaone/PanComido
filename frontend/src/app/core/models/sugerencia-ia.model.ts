export interface IngredienteSugeridoIA {
  insumoId: number;
  nombre: string;
  cantidad: number;
}

export interface PlatoSugeridoIA {
  id: number;
  nombre: string;
  descripcion: string;
  tiempoPreparacion: number;
  porcionesPosibles: number;
  ingredientesSugeridosIA: IngredienteSugeridoIA[];
}

export interface SugerenciaIA {
  fechaSugerencia: string;
  platosSugeridos: PlatoSugeridoIA[];
}