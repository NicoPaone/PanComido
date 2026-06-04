import { inject, Injectable, signal } from '@angular/core';
import { Comanda } from '../../../models/comanda/comanda';
import { SignalRConexionService } from '../base-hub-service';

@Injectable({
  providedIn: 'root',
})
export class ComandaHubService  {

  private conexion = inject(SignalRConexionService);
  comandaModificada = signal<Comanda | null>(null);

  public async conectarYUnirseGrupo(restauranteId:number):Promise<void> {
    await this.conexion.iniciar();
    
    await this.conexion.hub.invoke("UnirseCocina", restauranteId);
    this.conexion.hub.off("EstadoComandaModificada");

    this.conexion.hub.on("EstadoComandaModificada", (comanda: Comanda) => {
      this.comandaModificada.set(comanda);
    });
  }

  public async conectarComoMozo(restauranteId: number, mozoId: number): Promise<void> {
    await this.conexion.iniciar();
    await this.conexion.hub.invoke("UnirseMozo", restauranteId, mozoId);
    this.conexion.hub.off("EstadoComandaModificada");
    this.conexion.hub.on("EstadoComandaModificada", (comanda: Comanda) => {
        this.comandaModificada.set(comanda);
    });
  }

  public async conectarComoComensal(mesaId: number): Promise<void> {
    await this.conexion.iniciar();
    await this.conexion.hub.invoke("UnirseMesa", mesaId);
    this.conexion.hub.off("EstadoComandaModificada");
    this.conexion.hub.on("EstadoComandaModificada", (comanda: Comanda) => {
        this.comandaModificada.set(comanda);
    });
  }
  
  public detener():void{
    this.conexion.detener();
  }

  /** Remueve solo el listener de EsteComandaModificada — NO detiene el hub compartido */
  public desconectarEscucha(): void {
    this.conexion.hub.off("EstadoComandaModificada");
  }
 
}
