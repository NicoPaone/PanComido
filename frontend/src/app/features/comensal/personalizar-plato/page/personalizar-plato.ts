import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';
import { PlatoService } from '../../../../core/services/plato.service';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';


@Component({
  selector: 'app-personalizar-plato',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    FormsModule,
    LlamarAlMozo,
    BotonComensal
  ],
  templateUrl: './personalizar-plato.html',
  styleUrls: ['./personalizar-plato.css']
})
export class PersonalizarPlato implements OnInit {

  plato: any;
  configuracion = configuracionRestauranteMock;

  

  ingredientesRemover: string[] = [];

  extrasSeleccionados: string[] = [];
  removidosSeleccionados: string[] = [];

  observaciones = '';

  constructor(
    private router: Router,
    private pedidoService: PedidoService,
    private platoService: PlatoService
  ) {}

ngOnInit() {

  this.plato = history.state?.plato;

  console.log('PLATO:', this.plato);

  this.ingredientesRemover =
    this.plato?.plato?.receta?.map((i: any) => i.nombre) || [];

}

  volver() {
    this.router.navigate(['/comensal/pedido']);
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

    this.router.navigate(['/comensal/pedido']);

  }

}