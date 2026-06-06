import { Component, HostListener, inject, input, signal , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { Buscador } from '../../../../../app/shared/ui/buscador/buscador';
import { ListaPlatosComensalComponent } from '../components/lista-platos-comensal/lista-platos-comensal';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { PedidoState } from '../../services/pedido.state';
import { ItemPedido } from '../../../../core/models/domain/item-pedido';
import { configuracionRestauranteMock } from '../../../../../app/core/interceptors/handlers/configuracion-restaurante.mock';
import { CartaState } from '../service/carta-state';
import { ComensalFooterCart } from '../../components/comensal-footer-cart/comensal-footer-cart';
import { FiltrosCartaOverlay } from '../../components/filtros-carta-overlay/filtros-carta-overlay';
import { CommonModule } from '@angular/common';
import { ComensalState } from '../../services/comensal-state';


@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-ver-carta',
  standalone: true,
  imports: [
    CommonModule,
    ListaPlatosComensalComponent,
    Buscador,
    FontAwesomeModule,
    ComensalFooterCart,
    FiltrosCartaOverlay
  ],
  templateUrl: './ver-carta.html',
  styleUrls: ['./ver-carta.css'],
})
export class VerCarta {
  

  private router = inject(Router);
  private pedidoService = inject(PedidoState);
  state = inject(CartaState);
  comensalState = inject(ComensalState);

  logoUrl = input<string>('assets/images/logo/logo_el_ferroviario.png');

  mostrarFiltros = signal(false);
  faFilter = faFilter;
  configuracion = configuracionRestauranteMock;
  mesaId = signal(1);
  cantidadPersonas = signal(1);

  menuOrdenarAbierto = signal(false);
  ordenSeleccionado = signal('');

  // Usar computed del servicio para reactividad
  cantidadTotalPedido = this.pedidoService.cantidadTotal;
  totalPedido = this.pedidoService.totalPrecio;

  @HostListener('document:click', ['$event'])
  onClickOutSide(event: Event) {
    const target = event.target as HTMLElement;
    if(!target.closest('.dropdown-ordenar')){
      this.menuOrdenarAbierto.set(false);
    }
  }

  toggleMenuOrdenar(): void {
    this.menuOrdenarAbierto.update(v => !v);
  }

  seleccionarOrden(criterio: string, label: string): void {
    this.ordenSeleccionado.set(label);
    this.state.setOrdenar(criterio);
    this.menuOrdenarAbierto.set(false);
  }

  ngOnInit(): void {
    this.mesaId.set(history.state?.mesaId ?? 1);
    this.cantidadPersonas.set(history.state?.cantidadPersonas ?? 1);
    this.state.cargarCarta();
  }

  onSearch(valor: string): void {
    this.state.setBusqueda(valor);
  }

  irAPedido(): void {
    this.router.navigate(['/comensal/pedido']);
  }

  agregarAlPedido(item: ItemPedido): void {
    this.pedidoService.agregarPedido(item);
  }
}

