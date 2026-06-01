

import { UnidadMedida } from "../unidad-medida";
import { CategoriaInsumo } from "./categorias/categoria-insumo";


export interface Insumo{
    id: number;
    nombre: string;
    stockActual: number;
    vencimiento: string;
    unidadMedida: UnidadMedida;
    categoriaIngrediente: CategoriaInsumo;
    stockMinimo: number;

}
export interface InsumoResponseDto {
  id: number;
  nombre: string;
  stockActual: number;
  stockMinimo: number;
  vencimiento: string | null;
  unidadMedida: string; 
  categoria: string;   
  tipo: string;
  estadoStock: string;
}

export const INSUMOS_MOCK: Insumo[] = [
  {
    id: 1, 
    nombre: 'Ajo',
    stockActual: 5, 
    vencimiento: '2026-05-17',
    stockMinimo: 5,
    unidadMedida: { id: 1, nombre: 'Kg' },
    categoriaIngrediente: { id: 2, descripcion: 'Verdura', tipoAplica: 'Ingrediente' }
  },
  {
    id: 2, 
    nombre: 'Cebolla',
    stockActual: 25, 
    vencimiento: '2026-06-10',
    stockMinimo: 10,
    unidadMedida: { id: 1, nombre: 'Kg' },
    categoriaIngrediente: { id: 2, descripcion: 'Verdura', tipoAplica: 'Ingrediente' }
  },
  {
    id: 3, 
    nombre: 'Aceite de Girasol',
    stockActual: 3, 
    vencimiento: '2027-01-20',
    stockMinimo: 5,
    unidadMedida: { id: 3, nombre: 'Lt' },
    categoriaIngrediente: { id: 9, descripcion: 'Aceites y Grasas', tipoAplica: 'Ingrediente' }
  },
  {
    id: 4, 
    nombre: 'Harina 0000',
    stockActual: 1, 
    vencimiento: '2026-12-05',
    stockMinimo: 15,
    unidadMedida: { id: 1, nombre: 'Kg' },
    categoriaIngrediente: { id: 11, descripcion: 'Harinas y Panificados', tipoAplica: 'Ingrediente' }
  },
  {
    id: 5, 
    nombre: 'Tomate Perita',
    stockActual: 12, 
    vencimiento: '2026-05-25',
    stockMinimo: 5,
    unidadMedida: { id: 1, nombre: 'Kg' },
    categoriaIngrediente: { id: 2, descripcion: 'Verdura', tipoAplica: 'Ingrediente' }
  },
  {
    id: 6, 
    nombre: 'Bife de Chorizo',
    stockActual: 3, 
    vencimiento: '2026-05-28',
    stockMinimo: 10,
    unidadMedida: { id: 1, nombre: 'Kg' },
    categoriaIngrediente: { id: 3, descripcion: 'Carne', tipoAplica: 'Ingrediente' }
  }
];

