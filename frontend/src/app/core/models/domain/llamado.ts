export interface Llamado {
  id: number;
  mozoId: number | null;
  mesaId: number | null;
  gerenteId: number | null;
  categoriaLlamadoId: number;
  categoriaDescripcion: string;
  descripcion: string;
  resuelto: boolean;
}

export interface LlamadoMozo {
  mesaId: number;
  categoriaLlamadoId: number;
  descripcion: string;
}

export type TipoLlamado = 'admin' | 'cocina' | 'mesa';
