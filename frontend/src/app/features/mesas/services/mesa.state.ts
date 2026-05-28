import { App } from './../../../app';
import { Injectable, inject, signal } from '@angular/core';
import { MesaService } from '../../../core/services/mesa.service';
import { Mesa } from '../../../core/models/mesa.model';

@Injectable({ providedIn: 'root' })
export class MesaStateService {
  private api = inject(MesaService);

  // 1. Estado PRIVADO
  private _mesas = signal<Mesa[]>([]);
  private _loading = signal<boolean>(false);
  private _isEditorMode = signal<boolean>(false);

  // 2. Estado PÚBLICO (Solo lectura para los componentes)
  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  isEditorMode = this._isEditorMode.asReadonly();

  // 3. Casos de uso
  cargarMesas(): void {
    this._loading.set(true);
    this.api.getMesas().subscribe({
      next: (data) => {
        this._mesas.set(data);
        this._loading.set(false);
      },
      error: () => this._loading.set(false)
    });
  }

  toggleEditorMode(): void {
    this._isEditorMode.update(modo => !modo);
  }

  // Este método lo va a llamar el mapa cuando sueltes el clic del drag & drop
  moverMesa(id: number, deltaX: number, deltaY: number): void {
    const mesaActual = this._mesas().find(m => m.id === id);
    if (!mesaActual) return;

    // Calculamos las nuevas posiciones sumando la distancia que se movió (delta)
    const nuevosDatos: Partial<Mesa> = {
      posicionXInicio: mesaActual.posicionXInicio + deltaX,
      posicionXfin: mesaActual.posicionXfin + deltaX,
      posicionYinicio: mesaActual.posicionYinicio + deltaY,
      posicionYFin: mesaActual.posicionYFin + deltaY
    };

    // Optimistic Update: Mutamos el signal para que la mesa quede ahí al instante
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, ...nuevosDatos } : m)
    );

    // Mandamos el PUT al backend (o al mock en este caso) silenciosamente
    this.api.actualizarMesa(id, nuevosDatos).subscribe({
      error: () => {
        // Si el backend explota, revertimos el estado cargando la BD de nuevo
        console.error('Error al guardar la posición');
        this.cargarMesas();
      }
    });
  }
}
