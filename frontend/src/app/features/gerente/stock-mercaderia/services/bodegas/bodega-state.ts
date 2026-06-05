import { inject, Injectable, signal } from '@angular/core';
import { Bodega } from '../../../../../core/models/domain/bodega';
import { BodegaService } from './bodega-service';

@Injectable({
  providedIn: 'root',
})
export class BodegaState {

  private api = inject(BodegaService);

  private _bodegas = signal<Bodega[]>([]);

  bodegas = this._bodegas.asReadonly();

  cargarBodegas(): void {
    this.api.obtenerBodegas().subscribe({
      next: (data) => this._bodegas.set(data),

      error: (err) => void 0
    });
  }
  cargarBodegasConInsumos(): void {
    this.api.obtenerBodegasConInsumos().subscribe({
      next: (data) => this._bodegas.set(data),    
    });
  }

}

