import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';

@Component({
  selector: 'app-personalizar-plato',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    FormsModule,
    LlamarAlMozo
  ],
  templateUrl: './personalizar-plato.html',
  styleUrls: ['./personalizar-plato.css']
})
export class PersonalizarPlato implements OnInit {

  plato: any;
   configuracion = configuracionRestauranteMock;

  ingredientesExtra = [
    'Queso extra',
    'Panceta',
    'Huevo',
    'Palta',
    'Salsa picante'
  ];

  ingredientesRemover = [
    'Cebolla',
    'Tomate',
    'Mostaza',
    'Mayonesa'
  ];

  extrasSeleccionados: string[] = [];
  removidosSeleccionados: string[] = [];

  observaciones = '';

  constructor(
    private router: Router
  ) {}

  ngOnInit() {

    this.plato = history.state?.plato;

  }

  volver() {

    this.router.navigate([
      '/comensal/pedido'
    ]);

  }

  toggleExtra(ingrediente: string) {

    if (this.extrasSeleccionados.includes(ingrediente)) {

      this.extrasSeleccionados =
        this.extrasSeleccionados.filter(
          item => item !== ingrediente
        );

    } else {

      this.extrasSeleccionados.push(ingrediente);

    }

  }

  toggleRemover(ingrediente: string) {

    if (this.removidosSeleccionados.includes(ingrediente)) {

      this.removidosSeleccionados =
        this.removidosSeleccionados.filter(
          item => item !== ingrediente
        );

    } else {

      this.removidosSeleccionados.push(ingrediente);

    }

  }

  guardarCambios() {

    console.log({
      plato: this.plato,
      extras: this.extrasSeleccionados,
      remover: this.removidosSeleccionados,
      observaciones: this.observaciones
    });

    this.router.navigate([
      '/comensal/pedido'
    ]);

  }

}