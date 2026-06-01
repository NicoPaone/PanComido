import { Component, effect, inject, OnInit, OnDestroy } from '@angular/core';
import { ComandaState } from '../../services/comanda-state';
import { ComandaCard } from "../../components/comanda-card/comanda-card";
import { ComandoVoz, ComandoVozService } from '../../services/comando-voz/comando-voz.service';
import { BotonVoz } from '../../../../../shared/ui/botones/boton-voz/boton-voz';
import { ComandaHubService } from '../../../../../core/services/hubs/comanda/comanda-hub-service';

@Component({
  selector: 'app-comanda-page',
  standalone: true, // Asumo que es standalone por tu config
  imports: [ComandaCard, BotonVoz],
  templateUrl: './comanda-page.html',
  styleUrl: './comanda-page.css',
})
export class ComandaPage implements OnInit, OnDestroy {
  state = inject(ComandaState);
  vozService = inject(ComandoVozService);
  hub = inject(ComandaHubService);

  restauranteId = 1;

  constructor() {
    effect(() => {
      const comando: ComandoVoz | null = this.vozService.comandoDetectado();
      if(comando){
        this.state.modificarEstadoComanda(comando.comandaId, comando.nuevoEstadoId);
      }
    });

    effect(() => {
      const comandaPush = this.hub.comandaModificada();
      if(comandaPush){
        console.log('Comanda push:', comandaPush);
        this.state.actualizarDesdeHub(comandaPush);
        console.log("se actualizó")
      }
    });
  }

  ngOnInit() {
    this.state.cargarComandasActivas();
    this.hub.conectarYUnirseGrupo(this.restauranteId);
  }

  ngOnDestroy() {
    // Esto llamará a this.conexion.detener() internamente
    this.hub.detener();
  }

  procesarAccion(comandaId: number) {
    console.log('Disparando acción para comanda:', comandaId);
  }
}