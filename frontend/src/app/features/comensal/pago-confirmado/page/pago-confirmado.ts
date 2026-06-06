import { Component, inject , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaState } from '../../services/comanda-state';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-pago-confirmado',
  standalone: true,
  imports: [DecimalPipe, BotonComensal],
  templateUrl: './pago-confirmado.html',
  styleUrls: ['./pago-confirmado.css']
})
export class PagoConfirmado {
  private router = inject(Router);
  private comandaState = inject(ComandaState);

  configuracion = configuracionRestauranteMock;
  estado = this.comandaState.estadoPedido;
  mesaId = this.comandaState.mesaId;

  volverInicio(): void {
    this.comandaState.limpiarEstado();
    this.router.navigate(['/comensal/escanear-mesa']);
  }

  puntuarPlatos(): void {
    console.warn('puntuarPlatos no implementado aún');
  }
}
