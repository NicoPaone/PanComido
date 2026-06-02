import { Component, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Plato } from '../../../../../app/core/models/plato';
import { Buscador } from '../../../../../app/shared/ui/buscador/buscador';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { ListaPlatosComensalComponent } from '../components/lista-platos-comensal/lista-platos-comensal';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { PedidoService } from '../../../../../app/core/services/pedido.service';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { configuracionRestauranteMock } from '../../../../../app/core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PlatoService } from '../../../../core/services/plato.service';


@Component({
  selector: 'app-ver-carta',
  standalone: true,
  imports: [
    ListaPlatosComensalComponent,
    Buscador,
    BotonComensal,
    FontAwesomeModule,
    LlamarAlMozo
  ],
  templateUrl: './ver-carta.html',
  styleUrls: ['./ver-carta.css'],
})
export class VerCarta {
  private router = inject(Router);
  private pedidoService = inject(PedidoService);

  logoUrl = input<string>('assets/images/logo/logo_el_ferroviario.png');

  mostrarFiltros = signal(false);
  tiposSeleccionados = signal<string[]>([]);
  bebidasSeleccionadas = signal<string[]>([]);
  restriccionesSeleccionadas = signal<string[]>([]);
  faFilter = faFilter;
  tipoOrden = signal('');
  cantidadPersonas = signal(1);
  configuracion = configuracionRestauranteMock;
  filteredPlatos = signal<Plato[]>([]);
  platos = signal<Plato[]>([
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
  ]);

  constructor() {
    this.filteredPlatos.set([...this.platos()]);
  }

  onSearch(valor: string): void {
    this.filteredPlatos.set(
      this.platos().filter(plato =>
        plato.nombre.toLowerCase().includes(valor.toLowerCase())
      )
    );
  }

  irAPedido(): void {
    this.router.navigate(['/comensal/pedido']);
  }

  agregarAlPedido(item: ItemPedido): void {
    this.pedidoService.agregarPedido(item);
  }

  ordenar(tipo: string): void {
    this.tipoOrden.set(tipo);
    this.aplicarFiltros();
  }

  aplicarFiltros(): void {
    const filtrados = this.platos().filter(plato => {
      const tipos = this.tiposSeleccionados();
      const bebidas = this.bebidasSeleccionadas();
      const restricciones = this.restriccionesSeleccionadas();

      const filtroPlatoDelDia = tipos.includes('plato-del-dia');
      const tiposNormales = tipos.filter(t => t !== 'plato-del-dia');

      const cumpleTipo = tiposNormales.length === 0 || tiposNormales.includes(plato.tipo);
      const cumplePlatoDelDia = !filtroPlatoDelDia || plato.platoDelDia;
      const cumpleBebida = bebidas.length === 0 || bebidas.includes(plato.bebida);
      const cumpleRestriccion = restricciones.length === 0 || restricciones.includes(plato.restriccion);

      return cumpleTipo && cumplePlatoDelDia && cumpleBebida && cumpleRestriccion;
    });

    const orden = this.tipoOrden();
    switch (orden) {
      case 'precio-menor':
        filtrados.sort((a, b) => a.precioVenta - b.precioVenta);
        break;
      case 'precio-mayor':
        filtrados.sort((a, b) => b.precioVenta - a.precioVenta);
        break;
      case 'tiempo':
        filtrados.sort((a, b) => a.tiempo - b.tiempo);
        break;
      case 'nombre':
        filtrados.sort((a, b) => a.nombre.localeCompare(b.nombre));
        break;
      default:
        break;
    }

    this.filteredPlatos.set(filtrados);
    this.mostrarFiltros.set(false);
  }

  toggleFiltro(event: Event, lista: string[]): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      lista.push(checkbox.value);
    } else {
      const index = lista.indexOf(checkbox.value);
      if (index > -1) {
        lista.splice(index, 1);
      }
    }
  }

  mostrarTodos(event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      this.tiposSeleccionados.set([]);
      this.bebidasSeleccionadas.set([]);
      this.restriccionesSeleccionadas.set([]);
      this.tipoOrden.set('default');
      this.filteredPlatos.set([...this.platos()]);
    }
  }

  ngOnInit(): void {
    this.cantidadPersonas.set(history.state?.cantidadPersonas ?? 1);
  }

  get cantidadTotalPedido(): number {
    return this.pedidoService.obtenerPedidos().reduce(
      (total, item) => total + item.cantidad,
      0
    );
  }
}
