import { Component, output, inject } from '@angular/core';
import { CartaState } from '../../ver-carta/service/carta-state';

@Component({
  selector: 'app-filtros-carta-overlay',
  standalone: true,
  templateUrl: './filtros-carta-overlay.html',
  styleUrls: ['./filtros-carta-overlay.css']
})
export class FiltrosCartaOverlay {
  cartaState = inject(CartaState);
  cerrar = output<void>();

  limpiarFiltros(): void {
    this.cartaState.limpiarFiltros();
  }
}
