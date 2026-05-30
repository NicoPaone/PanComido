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
