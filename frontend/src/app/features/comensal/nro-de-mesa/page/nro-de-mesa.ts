import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { Router } from '@angular/router';
import { HeaderNroDeMesa } from '../components/header-nro-de-mesa/header-nro-de-mesa';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';


@Component({
  selector: 'app-nro-de-mesa',
  standalone: true,
    imports: [CommonModule, Boton, HeaderNroDeMesa, BotonComensal],
  templateUrl: './nro-de-mesa.html',
  styleUrls: ['./nro-de-mesa.css']
})
export class NroDeMesaComponent {
constructor(
  private router: Router
) {}

  numeroMesa = 12;
  configuracion = configuracionRestauranteMock;

  irACantidadPersonas() {

  this.router.navigate([
    '/comensal/cantidad-personas'
  ]);

}

volverAtras() {
  window.history.back();
}

}