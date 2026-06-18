export interface PorcentajeItem{
  id:number;
  descripcion: string;
  porcentaje: number;
}

export interface PorcentajesGanancia{
  platos: PorcentajeItem[];
  bebidas: PorcentajeItem[];
}