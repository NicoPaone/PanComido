import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { HeaderCantidadPersonas } from '../components/header-cantidad-personas/header-cantidad-personas';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';

@Component({
  selector: 'app-cantidad-personas',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    HeaderCantidadPersonas, 
    BotonComensal
  ],
  templateUrl: './cantidad-personas.html',
  styleUrls: ['./cantidad-personas.css']
})
export class CantidadPersonasComponent {

  cantidadPersonas = 1;
  maxCantidad = 5;
   configuracion = configuracionRestauranteMock;

  constructor(
    private router: Router
  ) { }

  expandirOpciones() {
    if (this.maxCantidad < 10) {
      this.maxCantidad = 10;
    }
  }

  seleccionarCantidad(numero: number) {
    this.cantidadPersonas = numero;
  }
  aceptar() {

    this.router.navigate(
      ['/comensal/ver-carta'],
      { state: { cantidadPersonas: this.cantidadPersonas } }
    );

  }

  volverAtras() {
    window.history.back();
  }


}