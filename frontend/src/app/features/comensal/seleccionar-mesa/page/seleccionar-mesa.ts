import { Component, inject, signal , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-seleccionar-mesa',
  standalone: true,
  imports: [CommonModule, BotonComensal],
  templateUrl: './seleccionar-mesa.html',
  styleUrls: ['./seleccionar-mesa.css']
})
export class SeleccionarMesa {
  private router = inject(Router);

  configuracion = configuracionRestauranteMock;
  mesaSeleccionada = signal<number | null>(null);

  // Lista de mesas disponibles (hardcodeado por ahora)
  mesas = [
    { id: 1, numero: 1 },
    { id: 2, numero: 2 },
    { id: 3, numero: 3 },
    { id: 4, numero: 4 },
    { id: 5, numero: 5 },
    { id: 6, numero: 6 }
  ];

  seleccionarMesa(mesaId: number): void {
    this.mesaSeleccionada.set(mesaId);
  }

  confirmarMesa(): void {
    const mesaId = this.mesaSeleccionada();

    if (!mesaId) {
      alert('Por favor, selecciona una mesa');
      return;
    }

    // Navegar a cantidad-personas (ahí se ocupa la mesa)
    this.router.navigate(['/comensal/cantidad-personas'], {
      state: { mesaId }
    });
  }
}
