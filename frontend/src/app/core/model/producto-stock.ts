
export type UnidadMedida= 'KG'| 'L' |'GR' | 'UN';
export type CategoriaIngrediente = 'Verdura' | 'Almacen' | 'Carne' | 'Lacteos'| 'Bebidas';

export interface ProductoStock{
    id: number;
    nombre: string;
    stock: number;
    fechaVencimiento: string;
    unidadMedida: UnidadMedida;
    categoriaIngrediente: CategoriaIngrediente;
    stockMinimo: number;

}

export const PRODUCTOS_STOCK_MOCK: ProductoStock[] = [
  {
    id: 1, 
    nombre: 'Ajo',
    stock: 5, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-05-17',
    stockMinimo: 5,
    categoriaIngrediente: 'Verdura'
  },
  {
    id: 2, 
    nombre: 'Cebolla',
    stock: 25, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-06-10',
    stockMinimo: 10,
    categoriaIngrediente: 'Verdura'
  },
  {
    id: 3, 
    nombre: 'Aceite de Girasol',
    stock: 3, 
    unidadMedida: 'L',
    fechaVencimiento: '2027-01-20',
    stockMinimo: 5,
    categoriaIngrediente: 'Almacen'
  },
  {
    id: 4, 
    nombre: 'Harina 0000',
    stock: 1, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-12-05',
    stockMinimo: 15,
    categoriaIngrediente: 'Almacen'
  },
  {
    id: 5, 
    nombre: 'Tomate Perita',
    stock: 12, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-05-25',
    stockMinimo: 5,
    categoriaIngrediente: 'Verdura'
  },
  {
    id: 6, 
    nombre: 'Bife de Chorizo',
    stock: 3, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-05-28',
    stockMinimo: 10,
    categoriaIngrediente: 'Carne'
  },
  {
    id: 7, 
    nombre: 'Huevos Blancos',
    stock: 120, 
    unidadMedida: 'UN',
    fechaVencimiento: '2026-06-15',
    stockMinimo: 60,
    categoriaIngrediente: 'Almacen'
  },
  {
    id: 8, 
    nombre: 'Sal Fina',
    stock: 10, 
    unidadMedida: 'KG',
    fechaVencimiento: '2028-10-10',
    stockMinimo: 3,
    categoriaIngrediente: 'Almacen'
  },
  {
    id: 9, 
    nombre: 'Papa Negra',
    stock: 45, 
    unidadMedida: 'KG',
    fechaVencimiento: '2026-06-20',
    stockMinimo: 20,
    categoriaIngrediente: 'Verdura'
  },
  {
    id: 10, 
    nombre: 'Vinagre de Alcohol',
    stock: 8, 
    unidadMedida: 'L',
    fechaVencimiento: '2027-03-12',
    stockMinimo: 2,
    categoriaIngrediente: 'Almacen'
  },
  {
    id: 11, 
    nombre: 'Pimienta Negra en Grano',
    stock: 2, 
    unidadMedida: 'KG',
    fechaVencimiento: '2028-01-15',
    stockMinimo: 0.5,
    categoriaIngrediente: 'Almacen'
  }
];
