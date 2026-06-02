import { Component, effect, inject } from '@angular/core';
import { ComandaState } from '../../services/comanda-state';
import { ComandaCard } from "../../components/comanda-card/comanda-card";
import { ComandoVoz, ComandoVozService } from '../../services/comando-voz/comando-voz.service';
import { BotonVoz } from '../../../../../shared/ui/botones/boton-voz/boton-voz';
import { of } from 'rxjs';
import { ComandaHubService } from '../../../../../core/services/hubs/comanda/comanda-hub-service';

@Component({
  selector: 'app-comanda-page',
  imports: [ComandaCard, BotonVoz],
  templateUrl: './comanda-page.html',
  styleUrl: './comanda-page.css',
})
export class ComandaPage {
  state = inject(ComandaState);
  
  vozService = inject(ComandoVozService);

  hub = inject(ComandaHubService);

  //VENIR DE SESION o authstate??
  restauranteId = 1;

  constructor(){
    effect(() => {
      const comando: ComandoVoz | null = this.vozService.comandoDetectado();
      if(comando){
        this.state.modificarEstadoComanda(comando.comandaId,comando.nuevoEstadoId);
      }
    })
    effect(() => {
      const comandaPush = this.hub.comandaModificada();
      if(comandaPush){
        this.state.actualizarDesdeHub(comandaPush);
      }
    })
  }
  ngOnInit(){
    this.state.cargarComandasActivas();

    this.hub.conectarYUnirseGrupo(this.restauranteId);
  }
  ngOnDestroy() {
    this.hub.detener();
  }

  procesarAccion(evento: { mesaId: number; estadoId: number }): void {
  this.state.modificarEstadoComanda(evento.mesaId, evento.estadoId);
}
}
