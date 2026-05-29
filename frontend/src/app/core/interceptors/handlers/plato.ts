import { HttpRequest, HttpResponse, HttpHandlerFn } from "@angular/common/http";
import { delay, Observable, of } from "rxjs";
import { Plato } from "../../models/plato";
import { calcularCostoReceta } from "../../services/plato.service";

let dbPlatos: Plato[] = [
  {
    id: 1,
    nombre: 'Milanesa napolitana',
    precioVenta: 16200,
    costo: 13160,
    visible: true,
    recomendado: true,
    ventas: 150,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 1.5, unidadMedida: 'KG' },
      { id: '7', nombre: 'Huevos Blancos', cantidad: 6, unidadMedida: 'UN' },
      { id: '4', nombre: 'Harina 0000', cantidad: 1, unidadMedida: 'KG' },
      { id: '5', nombre: 'Tomate Perita', cantidad: 0.5, unidadMedida: 'KG' },
      { id: '1', nombre: 'Ajo', cantidad: 0.03, unidadMedida: 'KG' },
      { id: '8', nombre: 'Sal Fina', cantidad: 0.01, unidadMedida: 'KG' }
    ]
  },
  {
    id: 2,
    nombre: 'Porción de papas',
    precioVenta: 10000,
    costo: 7000,
    visible: true,
    recomendado: false,
    ventas: 300,
    categoria: 'Entradas',
    imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '9', nombre: 'Papa Negra', cantidad: 8, unidadMedida: 'KG' },
      { id: '3', nombre: 'Aceite de Girasol', cantidad: 0.4, unidadMedida: 'L' }
    ]
  },
  {
    id: 3,
    nombre: 'Pasta al pesto',
    precioVenta: 12600,
    costo: 8700,
    visible: true,
    recomendado: true,
    ventas: 80,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '4', nombre: 'Harina 0000', cantidad: 2, unidadMedida: 'KG' },
      { id: '7', nombre: 'Huevos Blancos', cantidad: 10, unidadMedida: 'UN' },
      { id: '3', nombre: 'Aceite de Girasol', cantidad: 0.5, unidadMedida: 'L' },
      { id: '1', nombre: 'Ajo', cantidad: 4.625, unidadMedida: 'KG' }
    ]
  },
  {
    id: 4,
    nombre: 'Pizza de muzarella',
    precioVenta: 12600,
    costo: 8700,
    visible: true,
    recomendado: false,
    ventas: 200,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '4', nombre: 'Harina 0000', cantidad: 2.5, unidadMedida: 'KG' },
      { id: '5', nombre: 'Tomate Perita', cantidad: 2, unidadMedida: 'KG' },
      { id: '3', nombre: 'Aceite de Girasol', cantidad: 1, unidadMedida: 'L' },
      { id: '2', nombre: 'Cebolla', cantidad: 3.888, unidadMedida: 'KG' }
    ]
  },
  {
    id: 5,
    nombre: 'Pastel de papa',
    precioVenta: 14800,
    costo: 9320,
    visible: true,
    recomendado: true,
    ventas: 120,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '9', nombre: 'Papa Negra', cantidad: 5, unidadMedida: 'KG' },
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 0.7, unidadMedida: 'KG' },
      { id: '2', nombre: 'Cebolla', cantidad: 0.07, unidadMedida: 'KG' },
      { id: '8', nombre: 'Sal Fina', cantidad: 0.0233, unidadMedida: 'KG' }
    ]
  },
  {
    id: 6,
    nombre: 'Pollo al curry',
    precioVenta: 19500,
    costo: 8600,
    visible: true,
    recomendado: false,
    ventas: 50,
    categoria: 'Principales',
    imagen: 'assets/pollo-al-curry.png',
    receta: [
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 1, unidadMedida: 'KG' },
      { id: '3', nombre: 'Aceite de Girasol', cantidad: 0.5, unidadMedida: 'L' },
      { id: '2', nombre: 'Cebolla', cantidad: 0.388, unidadMedida: 'KG' }
    ]
  },
  {
    id: 7,
    nombre: 'Solomillo de cerdo con salsa',
    precioVenta: 19460,
    costo: 10120,
    visible: false,
    recomendado: false,
    ventas: 10,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 1.2, unidadMedida: 'KG' },
      { id: '2', nombre: 'Cebolla', cantidad: 1.1, unidadMedida: 'KG' },
      { id: '1', nombre: 'Ajo', cantidad: 0.108, unidadMedida: 'KG' }
    ]
  },
  {
    id: 8,
    nombre: 'Risotto a la crema',
    precioVenta: 29460,
    costo: 20120,
    visible: false,
    recomendado: false,
    ventas: 5,
    categoria: 'Principales',
    imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=800&h=600',
    receta: [
      { id: '4', nombre: 'Harina 0000', cantidad: 5, unidadMedida: 'KG' },
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 2.2, unidadMedida: 'KG' },
      { id: '3', nombre: 'Aceite de Girasol', cantidad: 0.4, unidadMedida: 'L' },
      { id: '2', nombre: 'Cebolla', cantidad: 0.022, unidadMedida: 'KG' }
    ]
  },
  {
    id: 9,
    nombre: 'Gaseosa Cola',
    precioVenta: 2000,
    costo: 800,
    visible: true,
    recomendado: false,
    ventas: 500,
    categoria: 'Bebidas',
    imagen: 'https://images.unsplash.com/photo-1622483767028-3f66f32aef97?auto=format&fit=crop&q=80&w=800&h=600',
    receta: []
  }
];

export const handlePlatoMock = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<any> => {
  const method = req.method;
  const url = req.url;

  if (method === 'GET') {
    return of(new HttpResponse({ status: 200, body: [...dbPlatos] })).pipe(delay(200));
  }

  if (method === 'POST') {
    const nuevo = req.body as Omit<Plato, 'id'>;
    const body: Plato = {
      ...nuevo,
      id: dbPlatos.length > 0 ? Math.max(...dbPlatos.map(p => p.id)) + 1 : 1
    };
    dbPlatos.push(body);
    return of(new HttpResponse({ status: 201, body })).pipe(delay(200));
  }

  if (method === 'PUT') {
    const id = parseInt(url.split('/').pop() || '0', 10);
    const index = dbPlatos.findIndex(p => p.id === id);
    if (index !== -1) {
      const updatedData = req.body as any;
      const merged = { ...dbPlatos[index], ...updatedData };
      if (updatedData.receta) {
        merged.costo = calcularCostoReceta(updatedData.receta);
      }
      dbPlatos[index] = merged;
      return of(new HttpResponse({ status: 200, body: merged })).pipe(delay(200));
    }
  }

  if (method === 'DELETE') {
    const id = parseInt(url.split('/').pop() || '0', 10);
    dbPlatos = dbPlatos.filter(p => p.id !== id);
    return of(new HttpResponse({ status: 204 })).pipe(delay(200));
  }

  return next(req);
};
