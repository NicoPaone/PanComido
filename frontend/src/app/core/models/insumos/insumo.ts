
export type UnidadMedida= 'KG'| 'L' |'GR' | 'UN' | '-';
export type CategoriaIngrediente = 'Verdura' | 'Almacen' | 'Carne' | 'Lacteos'| 'Bebidas' | 'Sin alcohol';


// [
//   {
//     "id": 12,
//     "nombre": "Coca-Cola 500ml",
//     "stockActual": 24,
//     "unidadMedida": "-",
//     "vencimiento": null,
//     "stockMinimo": 5,
//     "estadoStock": "Normal",
//     "tipo": "Bebida",
//     "categoria": "Sin alcohol"
//   },
export interface Insumo {
    id: number;
    nombre: string;
    stockActual: number;        
    unidadMedida: UnidadMedida | string;
    vencimiento: string | null;  
    stockMinimo: number;
    estadoStock: string;        
    tipo: string;             
    categoria: string;          
}

export const INSUMOS_MOCK: Insumo[] = [
  {
    id: 1, 
    nombre: 'Ajo',
    stockActual: 5, 
    unidadMedida: 'KG',
    vencimiento: '2026-05-17',
    stockMinimo: 5,
    estadoStock: 'Normal',
    tipo: 'Verdura',
    categoria: 'Verdura'
  },
  {
    id: 2, 
    nombre: 'Cebolla',
    stockActual: 25, 
    unidadMedida: 'KG',
    vencimiento: '2026-06-10',
    stockMinimo: 10,
    estadoStock: 'Normal',
    tipo: 'Verdura',
    categoria: 'Verdura'
  },
  {
    id: 3, 
    nombre: 'Aceite de Girasol',
    stockActual: 3, 
    unidadMedida: 'L',
    vencimiento: '2027-01-20',
    stockMinimo: 5,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Almacen'
  },
  {
    id: 4, 
    nombre: 'Harina 0000',
    stockActual: 1, 
    unidadMedida: 'KG',
    vencimiento: '2026-12-05',
    stockMinimo: 15,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Almacen'
  },
  {
    id: 5, 
    nombre: 'Tomate Perita',
    stockActual: 12, 
    unidadMedida: 'KG',
    vencimiento: '2026-05-25',
    stockMinimo: 5,
    estadoStock: 'Normal',
    tipo: 'Verdura',
    categoria: 'Verdura'
  },
  {
    id: 6, 
    nombre: 'Bife de Chorizo',
    stockActual: 3, 
    unidadMedida: 'KG',
    vencimiento: '2026-05-28',
    stockMinimo: 10,
    estadoStock: 'Normal',
    tipo: 'Carne',
    categoria: 'Carne'
  }
];
