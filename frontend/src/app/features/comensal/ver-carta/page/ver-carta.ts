import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Plato } from '../../../../../app/core/models/plato';
import { Buscador } from '../../../../../app/shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';

import { ListaPlatosComensalComponent } from '../components/lista-platos-comensal/lista-platos-comensal';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';

import { Router } from '@angular/router';
import { PedidoService } from '../../../../../app/core/services/pedido.service';
import { FormsModule } from '@angular/forms';
import { ItemPedido } from '../../../../core/models/item-pedido';

@Component({
  selector: 'app-ver-carta',
  standalone: true,
  imports: [
    CommonModule,
    ListaPlatosComensalComponent,
    Buscador,
    Boton,
    FontAwesomeModule,
    FormsModule
  ],
  templateUrl: './ver-carta.html',
  styleUrls: ['./ver-carta.css'],

})
export class VerCartaComponent {

  constructor(
    private router: Router,
    private pedidoService: PedidoService
    
  ) { 
    
  }

  mostrarFiltros = false;
  tiposSeleccionados: string[] = [];
  bebidasSeleccionadas: string[] = [];
  restriccionesSeleccionadas: string[] = [];
  faFilter = faFilter;
  tipoOrden: string = '';
  cantidadPersonas: number = 1;

  platos: Plato[] = [
    {
      id: 1,
      nombre: 'Milanesa napolitana',
      descripcion: 'Milanesa de carne con salsa de tomate, jamón y mozzarella gratinada.',
      platoDelDia: true,
      precioVenta: 16200,
      costo: 13160,
      tiempo: 25,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: true,
      imagen: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQKEGXoEhw1noD0K7RBypJC7RrtSX8V42ps2wJ8YgLjagQW_Rn9hnRMM4LFO1cUp0UWLnirJ_JWFHd07pehskFg0VSKOYcQ-ArTILAfLQ&s=10&w=200&h=150'
    },
    {
      id: 2,
      nombre: 'Porción de papas',
      descripcion: 'Papas fritas crocantes acompañadas con salsa especial de la casa.',
      platoDelDia: false,
      precioVenta: 10000,
      costo: 7000,
      tiempo: 15,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 3,
      nombre: 'Pasta al pesto',
      descripcion: 'Pasta artesanal con salsa pesto de albahaca fresca y queso parmesano.',
      platoDelDia: false,
      precioVenta: 12600,
      costo: 8700,
      tiempo: 20,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 4,
      nombre: 'Pizza de muzarella',
      descripcion: 'Pizza clásica con abundante mozzarella y salsa de tomate casera.',
      platoDelDia: false,
      precioVenta: 12600,
      costo: 8700,
      tiempo: 25,
      tipo: 'principal',
      bebida: '',
      restriccion: 'vegetariano',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 5,
      nombre: 'Pastel de papa',
      descripcion: 'Pastel casero de carne condimentada y puré de papas gratinado.',
      platoDelDia: false,
      precioVenta: 14800,
      costo: 9320,
      tiempo: 30,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 6,
      nombre: 'Pollo al curry',
      descripcion: 'Pollo cocinado en salsa curry suave acompañado con arroz especiado.',
      platoDelDia: false,
      precioVenta: 19500,
      costo: 8600,
      tiempo: 25,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: true,
    imagen: 'https://i.blogs.es/8c3360/pollo_curry/840_560.jpg?w=200&h=150'
    },
    {
      id: 7,
      nombre: 'Solomillo de cerdo con salsa',
      descripcion: 'Solomillo de cerdo tierno servido con salsa cremosa y vegetales.',
      platoDelDia: false,
      precioVenta: 19460,
      costo: 10120,
      tiempo: 45,
      tipo: 'principal',
      bebida: '',
      restriccion: '',
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 8,
      nombre: 'Risotto a la crema',
      descripcion: 'Risotto cremoso preparado con queso parmesano y hierbas frescas.',
      platoDelDia: false,
      precioVenta: 29460,
      costo: 20120,
      tiempo: 30,
      tipo: 'principal',
      bebida: '',
      restriccion: 'vegetariano',
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=150'
    }
  ];

  filteredPlatos: Plato[] = this.platos;

  onSearch(valor: string) {

    this.filteredPlatos = this.platos.filter(plato =>
      plato.nombre.toLowerCase().includes(valor.toLowerCase())
    );

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

  this.filteredPlatos = this.platos.filter(plato => {

    const filtroPlatoDelDia =
      this.tiposSeleccionados.includes(
        'plato-del-dia'
      );

    const tiposNormales =
      this.tiposSeleccionados.filter(
        tipo => tipo !== 'plato-del-dia'
      );

    const cumpleTipo =

      tiposNormales.length === 0 ||

      tiposNormales.includes(plato.tipo);

    const cumplePlatoDelDia =

      !filtroPlatoDelDia ||

      plato.platoDelDia;

    const cumpleBebida =

      this.bebidasSeleccionadas.length === 0 ||

      this.bebidasSeleccionadas.includes(
        plato.bebida
      );

    const cumpleRestriccion =

      this.restriccionesSeleccionadas.length === 0 ||

      this.restriccionesSeleccionadas.includes(
        plato.restriccion
      );

    return (
      cumpleTipo &&
      cumplePlatoDelDia &&
      cumpleBebida &&
      cumpleRestriccion
    );

  });

  // ORDENAR DESPUÉS DE FILTRAR

  switch (this.tipoOrden) {

    case 'precio-menor':

      this.filteredPlatos.sort(
        (a, b) => a.precioVenta - b.precioVenta
      );

      break;

    case 'precio-mayor':

      this.filteredPlatos.sort(
        (a, b) => b.precioVenta - a.precioVenta
      );

      break;

    case 'tiempo':

      this.filteredPlatos.sort(
        (a, b) => a.tiempo - b.tiempo
      );

      break;

    case 'nombre':

      this.filteredPlatos.sort(
        (a, b) => a.nombre.localeCompare(b.nombre)
      );

      break;

    case 'default':

    default:

      this.filteredPlatos = [
        ...this.filteredPlatos
      ];

      break;

  }

  this.mostrarFiltros = false;

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

  }

}

ngOnInit() {
  this.cantidadPersonas = history.state?.cantidadPersonas ?? 1;
}

}