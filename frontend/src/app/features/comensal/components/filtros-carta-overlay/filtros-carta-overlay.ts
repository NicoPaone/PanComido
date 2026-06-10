import { Component, output, inject, signal } from '@angular/core';
import { CartaState } from '../../ver-carta/service/carta-state';
import {Boton} from "../../../../shared/ui/botones/boton/boton";

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-filtros-carta-overlay',
  standalone: true,
  templateUrl: './filtros-carta-overlay.html',
  styleUrls: ['./filtros-carta-overlay.css'],
  imports: [Boton]
})
export class FiltrosCartaOverlay {
  cantidadFiltrosActivos = input<number>(0);
  tiposSeleccionados = input<string[]>([]);
  tieneFiltrosActivos = input<boolean>(false);

  cerrar = output<void>();
  toggleTipo = output<string>();
  limpiar = output<void>();

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
    this.limpiar.emit();
  }
}
