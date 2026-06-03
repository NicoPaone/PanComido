import { Component, output, inject, signal } from '@angular/core';
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

  seccionPlatosAbierta = signal(true);
  seccionBebidasAbierta = signal(true);
  seccionRestriccionesAbierta = signal(true);

  toggleSeccion(seccion: 'platos' | 'bebidas' | 'restricciones'): void {
    if (seccion === 'platos') this.seccionPlatosAbierta.update(v => !v);
    if (seccion === 'bebidas') this.seccionBebidasAbierta.update(v => !v);
    if (seccion === 'restricciones') this.seccionRestriccionesAbierta.update(v => !v);
  }

  aplicarFiltros(): void {
    this.cerrar.emit();
  }

  limpiarFiltros(): void {
    this.cartaState.limpiarFiltros();
  }
}
