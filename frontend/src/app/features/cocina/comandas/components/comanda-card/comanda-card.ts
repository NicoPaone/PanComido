import { Component, computed, input, output } from '@angular/core';
import { Comanda } from '../../../../../core/models/comanda/comanda';

@Component({
  selector: 'app-comanda-card',
  imports: [],
  templateUrl: './comanda-card.html',
  styleUrl: './comanda-card.css',
})
export class ComandaCard {


  comanda = input.required<Comanda>();
  accion = output<number>();


  //VER COLORES
  headerClass = computed(() => {
    const estado = this.comanda().estado;
    if (estado === 'EnPreparacion') return 'bg-danger'; 
    if (estado === 'Nueva') return 'bg-success'; 
    if (estado === 'EnEspera') return 'bg-warning'; 
    return 'bg-gray';
  });

  textoBoton = computed(() => {
    return this.comanda().estado === 'Nueva' ? 'ACEPTAR COMANDA' : 'LLAMAR MOZO';
  });

}
