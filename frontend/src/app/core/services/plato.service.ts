import { Injectable, signal } from '@angular/core';
import { Plato, RecetaIngrediente } from '../models/plato';
import { Observable, of, delay } from 'rxjs';
import { environment } from '../../../environments/environment';

export const PLATO_ENDPOINTS = {
  base: `${environment.apiUrl}/platos`,
  crear: `${environment.apiUrl}/platos`,
  actualizar: (id: number) => `${environment.apiUrl}/platos/${id}`,
  eliminar: (id: number) => `${environment.apiUrl}/platos/${id}`
};

const COSTOS_INGREDIENTES: Record<string, number> = {
  '1': 1200,
  '2': 900,
  '3': 1500,
  '4': 600,
  '5': 1100,
  '6': 7500,
  '7': 120,
  '8': 300,
  '9': 800,
  '10': 700,
  '11': 4500
};

export function calcularCostoReceta(receta: RecetaIngrediente[]): number {
  return receta.reduce((total, ing) => {
    const costoUnitario = COSTOS_INGREDIENTES[ing.id] || 500;
    return total + (costoUnitario * ing.cantidad);
  }, 0);
}

@Injectable({
  providedIn: 'root'
})
export class PlatoService {
  private platosList = signal<Plato[]>([
    {
      id: 1,
      nombre: 'Milanesa napolitana',
      precioVenta: 16200,
      costo: 13160,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1565557612662-811c7504ee42?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=200&h=150',
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
      imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=150',
      receta: [
        { id: '4', nombre: 'Harina 0000', cantidad: 5, unidadMedida: 'KG' },
        { id: '6', nombre: 'Bife de Chorizo', cantidad: 2.2, unidadMedida: 'KG' },
        { id: '3', nombre: 'Aceite de Girasol', cantidad: 0.4, unidadMedida: 'L' },
        { id: '2', nombre: 'Cebolla', cantidad: 0.022, unidadMedida: 'KG' }
      ]
    }
  ]);

  getPlatos(): Observable<Plato[]> {
    // NOTE: El endpoint del back para listar platos debe conectarse aquí
    return of(this.platosList()).pipe(delay(200));
  }

  crearPlato(plato: Omit<Plato, 'id'>): Observable<Plato> {
    const nuevoPlato: Plato = {
      ...plato,
      id: Date.now()
    };
    this.platosList.update(platos => [...platos, nuevoPlato]);
    // NOTE: El endpoint del back para crear un plato nuevo debe conectarse aquí
    return of(nuevoPlato).pipe(delay(200));
  }

  updatePlato(id: number, updatedData: Partial<Plato>): Observable<Plato> {
    this.platosList.update(platos =>
      platos.map(p => {
        if (p.id === id) {
          const merged = { ...p, ...updatedData };
          if (updatedData.receta) {
            merged.costo = calcularCostoReceta(updatedData.receta);
          }
          return merged;
        }
        return p;
      })
    );
    const updated = this.platosList().find(p => p.id === id)!;
    // NOTE: El endpoint del back para actualizar platos debe conectarse aquí
    return of(updated).pipe(delay(200));
  }

  deletePlato(id: number): Observable<boolean> {
    this.platosList.update(platos => platos.filter(p => p.id !== id));
    // NOTE: El endpoint del back para borrar platos debe conectarse aquí
    return of(true).pipe(delay(200));
  }
}
