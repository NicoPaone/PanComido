import { Injectable, signal } from '@angular/core';
import { Plato } from '../models/plato';
import { Observable, of, delay } from 'rxjs';

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
      imagen: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 2,
      nombre: 'Porción de papas',
      precioVenta: 10000,
      costo: 7000,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 3,
      nombre: 'Pasta al pesto',
      precioVenta: 12600,
      costo: 8700,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 4,
      nombre: 'Pizza de muzarella',
      precioVenta: 12600,
      costo: 8700,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 5,
      nombre: 'Pastel de papa',
      precioVenta: 14800,
      costo: 9320,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 6,
      nombre: 'Pollo al curry',
      precioVenta: 19500,
      costo: 8600,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1565557612662-811c7504ee42?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 7,
      nombre: 'Solomillo de cerdo con salsa',
      precioVenta: 19460,
      costo: 10120,
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 8,
      nombre: 'Risotto a la crema',
      precioVenta: 29460,
      costo: 20120,
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=150'
    }
  ]);

  getPlatos(): Observable<Plato[]> {
    return of(this.platosList()).pipe(delay(200));
  }

  updatePlato(id: number, updatedData: Partial<Plato>): Observable<Plato> {
    this.platosList.update(platos =>
      platos.map(p => p.id === id ? { ...p, ...updatedData } : p)
    );
    const updated = this.platosList().find(p => p.id === id)!;
    return of(updated).pipe(delay(200));
  }

  deletePlato(id: number): Observable<boolean> {
    this.platosList.update(platos => platos.filter(p => p.id !== id));
    return of(true).pipe(delay(200));
  }
}
