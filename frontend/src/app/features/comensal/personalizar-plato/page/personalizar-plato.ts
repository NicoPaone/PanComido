import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';
import { PlatoService } from '../../../../core/services/plato.service';
import { ApiClient } from '../../../../core/services/api-client';
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
    , private api: ApiClient
  ) {}

ngOnInit() {

  this.plato = history.state?.plato;

  console.log('PLATO:', this.plato);

  // Intentar obtener ingredientes desde el objeto (mocks) primero
  this.ingredientesRemover =
    this.plato?.plato?.receta?.map((i: any) => i.nombre) || [];

  // Si no hay receta (por ejemplo cuando la carta viene desde la API), pedir detalle del artículo
  if ((!this.ingredientesRemover || this.ingredientesRemover.length === 0) && this.plato?.plato?.id) {
    this.api.get<any>(`articulo/${this.plato.plato.id}`).subscribe(det => {
      const opciones = det?.ingredientesOpcionales ?? det?.IngredientesOpcionales ?? [];
      this.ingredientesRemover = opciones.map((o: any) => o?.nombre ?? o?.Nombre ?? o?.nombreIngrediente ?? 'Ingrediente');
    }, err => {
      console.warn('No se pudo obtener detalle del artículo', err);
    });
  }

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