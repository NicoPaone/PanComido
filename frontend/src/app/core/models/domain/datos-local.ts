export interface DatosLocal {
  id: number;
  nombre: string;
  imagen: string | null;
  colorPrincipal: string | null;
  colorSecundario: string | null;
  textoPrincipal: string | null;
  textoSecundario: string | null;
  direccion: string;
}

export type DatosLocalEditables = Pick<DatosLocal,
'nombre' | 
'imagen' | 
'colorPrincipal' | 
'colorSecundario' | 
'textoPrincipal' | 
'textoSecundario'
>;