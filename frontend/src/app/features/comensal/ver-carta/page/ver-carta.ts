import { Component, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Buscador } from '../../../../../app/shared/ui/buscador/buscador';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { ListaPlatosComensalComponent } from '../components/lista-platos-comensal/lista-platos-comensal';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { PedidoService } from '../../../../../app/core/services/pedido.service';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { configuracionRestauranteMock } from '../../../../../app/core/interceptors/handlers/configuracion-restaurante.mock';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { CartaState } from '../service/carta-state';

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
  state = inject(CartaState);

  logoUrl = input<string>('assets/images/logo/logo_el_ferroviario.png');

  mostrarFiltros = signal(false);
  faFilter = faFilter;
  configuracion = configuracionRestauranteMock;
  cantidadPersonas = signal(1);

  ngOnInit(): void {
    this.cantidadPersonas.set(history.state?.cantidadPersonas ?? 1);
    this.state.cargarCarta(1); // restauranteId hardcodeado por ahora
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

  ordenar(tipo: string): void {
    this.state.setOrdenar(tipo);
  }

  get cantidadTotalPedido(): number {
    return this.pedidoService.obtenerPedidos().reduce(
      (total, item) => total + item.cantidad,
      0
    );
  }
  toggleEnArray(lista: string[], event: Event): string[] {
  const checkbox = event.target as HTMLInputElement;
  if (checkbox.checked) {
    return [...lista, checkbox.value];
  }
  return lista.filter(v => v !== checkbox.value);
}
}
