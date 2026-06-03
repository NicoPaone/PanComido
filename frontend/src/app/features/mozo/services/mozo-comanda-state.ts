import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { MozoComandaService } from './mozo-comanda-service';
import { Comanda } from '../../../core/models/comanda/comanda';
import { forkJoin } from 'rxjs';
import { ComandaHubService } from '../../../core/services/hubs/comanda/comanda-hub-service';
import { EstadoComanda } from '../../../core/models/comanda/comanda';

@Injectable({
  providedIn: 'root',
})
export class MozoComandaState {

  private api = inject(MozoComandaService);
  private hub = inject(ComandaHubService);
  private _comandas = signal<Comanda[]>([]);
  private _cargando = signal<boolean>(false);

  comandas = this._comandas.asReadonly();
  cargando = this._cargando.asReadonly();

  comandasNuevas = computed(() => 
  this._comandas().filter(c => c.estado === 'Nueva'));

  comandasEnPreparacion = computed(() =>
  this._comandas().filter(c => c.estado === 'EnPreparacion'));

  comandasEnEspera = computed(() =>
  this._comandas().filter(c => c.estado === 'EnEspera'));

readonly #hubEffect = effect(() => {
    const actualizada = this.hub.comandaModificada();
    if (!actualizada) return;

    const normalizada = {
        ...actualizada,
        estado: actualizada.estado.replace(/\s/g, '') as EstadoComanda
    };

    this._comandas.update(lista => {
        const estadosFinales = ['Finalizada', 'Abierta'];
        if (estadosFinales.includes(normalizada.estado)) {
            return lista.filter(c => c.id !== normalizada.id);
        }
        const existe = lista.some(c => c.id === normalizada.id);
        if (existe) {
            return lista.map(c => c.id === normalizada.id ? normalizada : c);
        }
        return [normalizada, ...lista];
    });
});

  cargarComandas(): void{ 
    this._cargando.set(true);
    this.api.listarComandas().subscribe({
      next: (comandas) => {
        this._comandas.set(comandas);
        console.log('Comandas cargadas:', comandas);
        this._cargando.set(false);
      },
      error: (error) => {
        console.error('Error al cargar comandas:', error);
        this._cargando.set(false);
      }
    });
  }

  entregarItems(comandaId: number, articuloComandaIds: number[]): void {
    if (articuloComandaIds.length === 0) return;

    this.api.entregarItems(comandaId, articuloComandaIds).subscribe({
      next: (comandaActualizada) => {
        this._comandas.update(lista =>
          lista.map(c => c.id === comandaActualizada.id ? comandaActualizada : c)
        );
      },
      error: (err) => console.error('Error al entregar items', err)
    });
  }
 
  async conectarHub(restauranteId: number, mozoId: number): Promise<void> {
    await this.hub.conectarComoMozo(restauranteId, mozoId);
  }
}
