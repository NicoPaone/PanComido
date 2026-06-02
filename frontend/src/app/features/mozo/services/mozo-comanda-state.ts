import { computed, inject, Injectable, signal } from '@angular/core';
import { MozoComandaService } from './mozo-comanda-service';
import { Comanda } from '../../../core/models/comanda/comanda';
import { forkJoin } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MozoComandaState {

  private api = inject(MozoComandaService)
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
  // entregarItems(comandaId: number, articuloComandaIds: number[]): void {
  //    if (articuloComandaIds.length === 0) return;

  //   const peticiones = articuloComandaIds.map(id =>
  //     this.api.marcarItemEntregado(comandaId, id)
  //   );

  //   forkJoin(peticiones).subscribe({
  //     next: () => this.cargarComandas(),
  //     error: (err) => console.error('Error al entregar items', err)
  //   });
  // }
}
