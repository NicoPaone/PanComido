export interface IngredienteSugerido {
  insumoId: number;
  nombre: string;
  cantidad: number;
}

export interface PlatoSugerido {
  id: number;
  nombre: string;
  descripcion: string;
  tiempoPreparacion: number;
  porcionesPosibles: number;
  ingredientesSugeridos: IngredienteSugerido[];
}

export interface Sugerencia {
  fechaSugerencia: string;
  platosSugeridos: PlatoSugerido[];
}
