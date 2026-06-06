import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Bodega } from '../../../../../core/models/domain/bodega';
import { BodegaService } from './bodega-service';

@Injectable({
  providedIn: 'root',
})
export class BodegaState {

  private api = inject(BodegaService);
  private destroyRef = inject(DestroyRef);

  private _bodegas = signal<Bodega[]>([]);

  bodegas = this._bodegas.asReadonly();

  cargarBodegas(): void {
    this.api.obtenerBodegas().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => this._bodegas.set(data),

      error: (err) => console.error('Error al cargar bodegas:', err)
    });
  }
  cargarBodegasConInsumos(): void {
    this.api.obtenerBodegasConInsumos().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (data) => this._bodegas.set(data),    
    });
  }

}

