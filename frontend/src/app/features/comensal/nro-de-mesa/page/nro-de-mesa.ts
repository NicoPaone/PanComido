import { Component, inject , ChangeDetectionStrategy} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { Router } from '@angular/router';
import { HeaderNroDeMesa } from '../components/header-nro-de-mesa/header-nro-de-mesa';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-nro-de-mesa',
  standalone: true,
  imports: [CommonModule, Boton, HeaderNroDeMesa, BotonComensal],
  templateUrl: './nro-de-mesa.html',
  styleUrls: ['./nro-de-mesa.css']
})
export class NroDeMesa {
  private router = inject(Router);


  mesaId: number = 1; //hardcodeado por ahora
  configuracion = configuracionRestauranteMock;

  irACantidadPersonas() {
    this.router.navigate(['/comensal/cantidad-personas'], {
      state: { mesaId: this.mesaId }
    });
  }

  volverAtras() {
    this.router.navigate(['/comensal/escanear-mesa']);
  }
}
