export interface DatosLocal {
  id: number;
  nombre: string;
  direccion: string;
  imagen: string | null;
  colorPrincipal: string | null;
  colorSecundario: string | null;
  textoPrincipal: string | null;
  textoSecundario: string | null;
  familiaTipograficaId: number | null;
}

export type DatosLocalEditables = Pick<DatosLocal,
'nombre' | 
'imagen' | 
'colorPrincipal' | 
'colorSecundario' | 
'textoPrincipal' | 
'textoSecundario' |
'familiaTipograficaId'
>;