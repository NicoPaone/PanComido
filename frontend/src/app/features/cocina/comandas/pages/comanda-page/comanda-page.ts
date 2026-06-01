import { Component, effect, inject } from '@angular/core';
import { ComandaState } from '../../services/comanda-state';
import { ComandaCard } from "../../components/comanda-card/comanda-card";
import { ComandoVozService } from '../../services/comando-voz/comando-voz.service';
import { BotonVoz } from '../../../../../shared/ui/botones/boton-voz/boton-voz';
import { of } from 'rxjs';

@Component({
  selector: 'app-comanda-page',
  imports: [ComandaCard, BotonVoz],
  templateUrl: './comanda-page.html',
  styleUrl: './comanda-page.css',
})
export class ComandaPage {
  state = inject(ComandaState);

  vozService = inject(ComandoVozService);

  ngOnInit(){
    this.state.cargarComandasActivas();
  }

  constructor(){
    effect(() => {
      const comando = this.vozService.comandoDetectado();
      if(comando){
        this.state.modificarEstadoComanda(comando.comandaId,comando.nuevoEstadoId);
      }
    })
  }
  procesarAccion(comandaId: number) {
    console.log('Disparando acción para comanda:', comandaId);
  }
}
