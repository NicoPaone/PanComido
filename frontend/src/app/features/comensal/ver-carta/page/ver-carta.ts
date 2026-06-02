import { Component, Input, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Plato } from '../../../../../app/core/models/plato';
import { Buscador } from '../../../../../app/shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { ListaPlatosComensalComponent } from '../components/lista-platos-comensal/lista-platos-comensal';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';

import { Router } from '@angular/router';
import { PedidoService } from '../../../../../app/core/services/pedido.service';
import { FormsModule } from '@angular/forms';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { configuracionRestauranteMock } from '../../../../../app/core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PlatoService } from '../../../../core/services/plato.service';


@Component({
  selector: 'app-ver-carta',
  standalone: true,
  imports: [
    CommonModule,
    ListaPlatosComensalComponent,
    Buscador,
    Boton,
    BotonComensal,
    FontAwesomeModule,
    FormsModule,
    LlamarAlMozo
  ],
  templateUrl: './ver-carta.html',
  styleUrls: ['./ver-carta.css'],

})
export class VerCarta {

  constructor(
    private router: Router,
    private pedidoService: PedidoService,
    private platoService: PlatoService,
    private changeDetectorRef: ChangeDetectorRef
  ) {

  }

  mostrarFiltros = false;
  tiposSeleccionados: string[] = [];
  bebidasSeleccionadas: string[] = [];
  restriccionesSeleccionadas: string[] = [];
  faFilter = faFilter;
  tipoOrden: string = '';
  cantidadPersonas: number = 1;
  configuracion = configuracionRestauranteMock;

  @Input() logoUrl: string = 'assets/images/logo/logo_el_ferroviario.png';

 
  platos: Plato[] = [];
  filteredPlatos: Plato[] = [];

ngOnInit() {

  this.cantidadPersonas = history.state?.cantidadPersonas ?? 1;

  this.platoService.getPlatos().subscribe(platos => {

    this.platos = [...platos];

    this.aplicarFiltros(); // 👈 IMPORTANTE

    this.changeDetectorRef.markForCheck();

  });

}

  onSearch(valor: string) {

    this.filteredPlatos = this.platos.filter(plato =>
      plato.nombre.toLowerCase().includes(valor.toLowerCase())
    );

    this.changeDetectorRef.markForCheck();

  }

  irAPedido() {
    this.router.navigate(['/comensal/pedido']);
  }

  agregarAlPedido(item: ItemPedido) {

    this.pedidoService.agregarPedido(item);

    console.log('Pedido agregado:', item.plato);

  }

  ordenActual = 'default';

  ordenar(tipo: string) {

    this.tipoOrden = tipo;

    this.aplicarFiltros();

  }


aplicarFiltros() {

  console.log(this.tiposSeleccionados);
  console.log(this.bebidasSeleccionadas);
  console.log(this.restriccionesSeleccionadas);

  const filtrados = this.platos.filter(plato => {

    const filtroPlatoDelDia =
      this.tiposSeleccionados.includes('plato-del-dia');

    const tiposNormales =
      this.tiposSeleccionados.filter(t => t !== 'plato-del-dia');

    const cumpleTipo =
      tiposNormales.length === 0 ||
      tiposNormales.includes(plato.tipo);

    const cumplePlatoDelDia =
      !filtroPlatoDelDia || plato.platoDelDia;

    const cumpleBebida =
      this.bebidasSeleccionadas.length === 0 ||
      this.bebidasSeleccionadas.includes(plato.bebida);

    const cumpleRestriccion =
      this.restriccionesSeleccionadas.length === 0 ||
      this.restriccionesSeleccionadas.includes(plato.restriccion);

    return (
      cumpleTipo &&
      cumplePlatoDelDia &&
      cumpleBebida &&
      cumpleRestriccion
    );

  });

  this.filteredPlatos = filtrados;

  switch (this.tipoOrden) {

    case 'precio-menor':
      this.filteredPlatos.sort((a, b) => a.precioVenta - b.precioVenta);
      break;

    case 'precio-mayor':
      this.filteredPlatos.sort((a, b) => b.precioVenta - a.precioVenta);
      break;

    case 'tiempo':
      this.filteredPlatos.sort((a, b) => a.tiempo - b.tiempo);
      break;

    case 'nombre':
      this.filteredPlatos.sort((a, b) => a.nombre.localeCompare(b.nombre));
      break;
  }

  this.mostrarFiltros = false;

  this.changeDetectorRef.markForCheck();
}
  toggleFiltro(
    event: Event,
    lista: string[]
  ) {

    const checkbox = event.target as HTMLInputElement;

    if (checkbox.checked) {

      if (!Array.isArray(lista)) {
        return;
      }

      lista.push(checkbox.value);

    } else {

      const index = lista.indexOf(checkbox.value);

      if (index > -1) {

        lista.splice(index, 1);

      }

    }

  }

  mostrarTodos(event: any) {

    if (event.target.checked) {

      this.tiposSeleccionados = [];

      this.bebidasSeleccionadas = [];

      this.restriccionesSeleccionadas = [];

      this.tipoOrden = 'default';

      this.filteredPlatos = [
        ...this.platos
      ];

      this.changeDetectorRef.markForCheck();

    }

  }


  get cantidadTotalPedido(): number {
    return this.pedidoService.obtenerPedidos().reduce(
      (total, item) => total + item.cantidad,
      0
    );
  }
}