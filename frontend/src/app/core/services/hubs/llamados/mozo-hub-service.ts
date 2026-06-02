import { inject, Injectable, signal } from '@angular/core';
import { SignalRConexionService } from '../base-hub-service';
import { Llamado } from '../../../models/llamados/llamado';


@Injectable({
  providedIn: 'root',
})

export class MozoHubService {
  private conexion = inject(SignalRConexionService);
  llamadoRecibido = signal<Llamado | null>(null);
  
  public async conectarYUnirseGrupo(restauranteId:number, mozoId:number, ): Promise<void> {
    await this.conexion.iniciar();
    await this.conexion.hub.invoke("UnirseMozo", restauranteId, mozoId);
    this.conexion.hub.off('LlamadoMozo');
    
    this.conexion.hub.on('LlamadoMozo', (llamado: Llamado) => {
      console.log("Llamado recibido en el hub: ", llamado);
      this.llamadoRecibido.set(llamado);
    });

    //VERIFICAR
    this.conexion.hub.on('LlamadoCocina', (llamado: Llamado) => {
      console.log("Llamado  cocina recibido en el hub: ", llamado); console.log("Llamado recibido en el hub: ", llamado);
      this.llamadoRecibido.set(llamado);
    });
  }
  public detener(): void {
    this.conexion.detener();
  }
}