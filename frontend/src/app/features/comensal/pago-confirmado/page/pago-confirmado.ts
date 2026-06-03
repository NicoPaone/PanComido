import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaStateService } from '../../services/comanda-state.service';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';

@Component({
  selector: 'app-pago-confirmado',
  standalone: true,
  imports: [DecimalPipe, BotonComensal],
  templateUrl: './pago-confirmado.html',
  styleUrls: ['./pago-confirmado.css']
})
export class PagoConfirmado {
  private router = inject(Router);
  private comandaState = inject(ComandaStateService);

  configuracion = configuracionRestauranteMock;
  estado = this.comandaState.estadoPedido;
  mesaId = this.comandaState.mesaId;

  volverInicio(): void {
    this.comandaState.limpiarEstado();
    this.router.navigate(['/comensal/escanear-mesa']);
  }

  puntuarPlatos(): void {
    // Para futura implementación
    console.log("Navegando a puntuar platos...");
  }
}
