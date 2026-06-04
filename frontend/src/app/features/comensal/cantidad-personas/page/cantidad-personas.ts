import { CommonModule } from '@angular/common';
import { Component, inject , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { HeaderCantidadPersonas } from '../components/header-cantidad-personas/header-cantidad-personas';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { ComandaState } from '../../services/comanda-state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-cantidad-personas',
  standalone: true,
  imports: [
    CommonModule,
    Boton,
    HeaderCantidadPersonas,
    BotonComensal
  ],
  templateUrl: './cantidad-personas.html',
  styleUrls: ['./cantidad-personas.css']
})
export class CantidadPersonas {
  private router = inject(Router);
  private comandaState = inject(ComandaState);

  cantidadPersonas = 1;
  maxCantidad = 5;
  configuracion = configuracionRestauranteMock;
  cargando = this.comandaState.cargando;

  // Viene del paso anterior (nro-de-mesa)
  mesaId: number = history.state?.mesaId ?? 1;

  expandirOpciones() {
    if (this.maxCantidad < 10) {
      this.maxCantidad = 10;
    }
  }

  seleccionarCantidad(numero: number) {
    this.cantidadPersonas = numero;
  }

  async aceptar() {
    try {
      await this.comandaState.ocuparMesa(this.mesaId, this.cantidadPersonas);
      this.router.navigate(['/comensal/ver-carta'], {
        state: { mesaId: this.mesaId, cantidadPersonas: this.cantidadPersonas }
      });
    } catch (error) {
      void 0;
    }
  }

  volverAtras() {
    this.router.navigate(['/comensal/nro-de-mesa'], {
      state: { mesaId: this.mesaId }
    });
  }
}
