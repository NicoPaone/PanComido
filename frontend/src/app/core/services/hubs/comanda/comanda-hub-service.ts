import { Injectable, inject, signal } from '@angular/core';
import { Comanda } from '../../../models/comanda/comanda';
import { SignalRConnectionService } from '../signal-r-conexion-service';

@Injectable({
  providedIn: 'root',
})
export class ComandaHubService {
  
  // 1. Inyectamos el Motor Único en lugar de heredar
  private conexion = inject(SignalRConnectionService);

  public comandaModificada = signal<Comanda | null>(null);

  public async conectarYUnirseGrupo(restauranteId: number): Promise<void> {
    // 2. Usamos el motor para iniciar (si ya está iniciado, la promesa se resuelve sola)
    await this.conexion.iniciar();
    
    // 3. Nos unimos a la "habitación" de C#
    await this.conexion.hub.invoke("UnirseCocina", restauranteId);

    // 4. FRANCOTIRADOR: .off() es vital. Evita que el evento se duplique 
    // si el usuario sale de la página de cocina y vuelve a entrar.
    this.conexion.hub.off("EstadoComandaModificada");
    
    // 5. Escuchamos el evento
    this.conexion.hub.on("EstadoComandaModificada", (data: Comanda) => {
      this.comandaModificada.set(data);
    });
  }

  public detener(): void {
    // Delegamos la acción de apagar la conexión al Motor
    this.conexion.detener();
  }
}