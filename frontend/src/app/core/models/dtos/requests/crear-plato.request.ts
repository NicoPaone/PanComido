export interface CrearPlatoRequestDto {
  nombre: string;
  descripcion: string;
  precioVentaFinal: number;
  tiempoPreparacionBase: number;
  tipoPlatoId: number;
  categoriaPlatoId: number;
  urlImagen: string;
  restriccionesIds: number[];
  ingredientes: CrearPlatoIngredienteDto[];
}

export interface CrearPlatoIngredienteDto {
  insumoId: number;
  cantidad: number;
  opcional: boolean;
}
