import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { HeaderCantidadPersonas } from '../components/header-cantidad-personas/header-cantidad-personas';

@Component({
  selector: 'app-cantidad-personas',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    HeaderCantidadPersonas
  ],
  templateUrl: './cantidad-personas.html',
  styleUrls: ['./cantidad-personas.css']
})
export class CantidadPersonasComponent {

  cantidadSeleccionada = 1;

  constructor(
    private router: Router
  ) {}

  seleccionarCantidad(
    cantidad: number
  ) {

    this.cantidadSeleccionada =
      cantidad;

  }

  aceptar() {

    this.router.navigate([
      '/comensal/ver-carta'
    ]);

  }

  volverAtras() {
  window.history.back();
}


}