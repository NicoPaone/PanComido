import { Component, inject } from '@angular/core';
import { ComandaState } from '../../services/comanda-state';
import { ComandaCard } from "../../components/comanda-card/comanda-card";

@Component({
  selector: 'app-comanda-page',
  imports: [ComandaCard],
  templateUrl: './comanda-page.html',
  styleUrl: './comanda-page.css',
})
export class ComandaPage {
  state = inject(ComandaState);

  ngOnInit(){
    this.state.cargarComandasActivas();
  }
  procesarAccion(comandaId: number) {
    console.log('Disparando acción para comanda:', comandaId);
  }
}
