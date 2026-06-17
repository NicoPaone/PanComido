import { DestroyRef, inject, Component, ChangeDetectionStrategy } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { HeaderCantidadPersonas } from '../components/header-cantidad-personas/header-cantidad-personas';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { configuracionRestauranteMock } from '../../../../infra/mocks/configuracion-restaurante.mock-data';
import { ComandaState } from '../../services/comanda-state';
import { ActivatedRoute } from '@angular/router';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-cantidad-personas',
  standalone: true,
  imports: [
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
  private destroyRef = inject(DestroyRef);
  private route = inject(ActivatedRoute);

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


aceptar() {
  console.log('route params:', this.route.snapshot.paramMap.keys);
  console.log('restauranteId:', this.route.snapshot.paramMap.get('restauranteId'));
  console.log('mesaId:', this.route.snapshot.paramMap.get('mesaId'));

  const restauranteId = Number(this.route.snapshot.paramMap.get('restauranteId'));
  const mesaId = Number(this.route.snapshot.paramMap.get('mesaId'));

  this.comandaState.ocuparMesa(restauranteId, mesaId, this.cantidadPersonas)
  .pipe(takeUntilDestroyed(this.destroyRef))
 .subscribe({
 next: (res: any) => {
  if (!res || !res.idComandaGenerada) {
    console.error('Respuesta inválida del backend', res);
    return;
  }

  sessionStorage.setItem('sesionComensal', JSON.stringify(res));

  this.router.navigate([
    '/comensal/ver-carta',
    restauranteId,
    mesaId,
    this.cantidadPersonas
  ]);
},
error: (err) => {
  console.error('Error ocupar mesa:', err);

  if (err.status === 409) {
    console.warn('Mesa ocupada, intentando recuperar sesión...');

    const sesion = sessionStorage.getItem('sesionComensal');

    if (sesion && sesion !== 'undefined') {
      const parsed = JSON.parse(sesion);

      this.router.navigate([
        '/comensal/ver-carta',
        parsed.restauranteId,
        parsed.mesaId,
        this.cantidadPersonas
      ]);
      return;
    }

    alert('Mesa ocupada pero no hay sesión válida');
  }
}
});

}

  volverAtras() {
    this.router.navigate(['/comensal/nro-de-mesa'], {
      state: { mesaId: this.mesaId }
    });
  }
}
