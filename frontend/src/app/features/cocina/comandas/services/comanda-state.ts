import { computed, inject, Injectable, signal } from '@angular/core';
import { ComandaService } from './comanda-service';
import { Comanda } from '../../../../core/models/comanda/comanda';

@Injectable({
  providedIn: 'root',
})
export class ComandaState {

  private api = inject(ComandaService);
  private _comandas = signal<Comanda[]>([]);
  private _cargando = signal<boolean>(false);

  comandas = this._comandas.asReadonly();
  cargando = this._cargando.asReadonly();


  comandasNuevas = computed(() => 
    this._comandas().filter(c => c.estado === 'Nueva')
  );
  
  comandasEnPreparacion = computed(() => 
    this._comandas().filter(c => c.estado === 'EnPreparacion')
  );
  
  comandasEnEspera = computed(() => 
    this._comandas().filter(c => c.estado === 'EnEspera')
  );
   comandasfinalizadas = computed(() => 
    this._comandas().filter(c => c.estado === 'Finalizada')
  );

  
  modificarEstadoComanda(comandaId: number, tipoId: number): void {
    this.api.modificarEstadoComanda(comandaId, tipoId).subscribe({
      next: (comandaActualizada) => {
        this._comandas.update(lista =>
          lista.map(c => c.id === comandaActualizada.id 
            ? comandaActualizada 
            : c
          )
        );
      },
    error: (err) => console.error('Error al modificar comanda', err)
  });
}
  marcarItemEntregado(comandaId: number, articuloComandaId: number): void {
  this.api.marcarItemEntregado(comandaId, articuloComandaId).subscribe({
    next: (comandaActualizada) => {
      this._comandas.update(lista =>
        lista.map(c => c.id === comandaActualizada.id ? comandaActualizada : c)
      );
    },
    error: (err) => console.error('Error al marcar item', err)
  });
}


  actualizarDesdeHub(comandaRecibida: Comanda): void{
    this._comandas.update(listaActual => {
      const existe = listaActual.some(comanda => comanda.id === comandaRecibida.id);
      if(existe){
        return listaActual.map(comanda => comanda.id === comandaRecibida.id ? comandaRecibida : comanda);
      } else{ 
        return [...listaActual, comandaRecibida];
      }
    })
  }

  cargarComandasActivas():void{
    this._cargando.set(true);
    this.api.obtenerComandasActivas().subscribe({
      next: (data) => {
        this._comandas.set(data);
        this._cargando.set(false);
      },
      error: () => this._cargando.set(false)
    })
  }

}
