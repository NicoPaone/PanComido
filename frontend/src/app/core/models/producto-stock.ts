
export type UnidadMedida= 'KG'| 'L' |'GR' | 'UN' | '-';
export type CategoriaIngrediente = 'Verdura' | 'Almacen' | 'Carne' | 'Lacteos'| 'Bebidas';


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
    categoria: 'Sin alcohol'
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
    categoria: 'Sin alcohol'
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
    categoria: 'Sin alcohol'
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
    categoria: 'Sin alcohol'
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
    categoria: 'Sin alcohol'
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
    categoria: 'Sin alcohol'
  },
  {
    id: 7, 
    nombre: 'Huevos Blancos',
    stockActual: 120, 
    unidadMedida: 'UN',
    vencimiento: '2026-06-15',
    stockMinimo: 60,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Sin alcohol'
  },
  {
    id: 8, 
    nombre: 'Sal Fina',
    stockActual: 10, 
    unidadMedida: 'KG',
    vencimiento: '2028-10-10',
    stockMinimo: 3,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Sin alcohol'
  },
  {
    id: 9, 
    nombre: 'Papa Negra',
    stockActual: 45, 
    unidadMedida: 'KG',
    vencimiento: '2026-06-20',
    stockMinimo: 20,
    estadoStock: 'Normal',
    tipo: 'Verdura',
    categoria: 'Sin alcohol'
  },
  {
    id: 10, 
    nombre: 'Vinagre de Alcohol',
    stockActual: 8, 
    unidadMedida: 'L',
    vencimiento: '2027-03-12',
    stockMinimo: 2,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Sin alcohol'
  },
  {
    id: 11, 
    nombre: 'Pimienta Negra en Grano',
    stockActual: 2, 
    unidadMedida: 'KG',
    vencimiento: '2028-01-15',
    stockMinimo: 0.5,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Sin alcohol'
  }
];
