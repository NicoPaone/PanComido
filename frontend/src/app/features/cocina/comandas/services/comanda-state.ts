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

  

  modificarEstadoComanda(mesaId:number,tipoId:number):void { 
    this.api.modificarEstadoComanda(mesaId,tipoId).subscribe({
    next: (comandaActualizada) => {

      this._comandas.update(comandasPrevias =>
        comandasPrevias.map(comanda =>  
         comanda.id === comandaActualizada.id 
         ? { 
            ...comanda,
             estado: comandaActualizada.estado, 
             estadoId: comandaActualizada.estadoId
           } 
           : comanda
      ))
    },error: (err) => {
        console.error('Error al modificar comanda', err);
      }
  
   });


  }
  actualizarDesdeHub(comandaRecibida: Comanda): void {
  this._comandas.update(listaActual => {
    // Verificamos si la comanda ya está en la pantalla
    const existe = listaActual.some(c => c.id === comandaRecibida.id);
    
    if (existe) {
      // Reemplazamos la vieja por la nueva
      return listaActual.map(c => c.id === comandaRecibida.id ? comandaRecibida : c);
    } else {
      // Es una comanda totalmente nueva, la agregamos al final (o al principio)
      return [...listaActual, comandaRecibida];
    }
  });
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
