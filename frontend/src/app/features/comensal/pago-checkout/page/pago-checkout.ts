import { Component, DestroyRef, inject, signal, effect, OnDestroy, OnInit , ChangeDetectionStrategy} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaState } from '../../services/comanda-state';
import { PagoService } from '../../services/pago.service';
import { ComandaHubService } from '../../../../core/services/hubs/comanda/comanda-hub-service';
import { configuracionRestauranteMock } from '../../../../infra/mocks/configuracion-restaurante.mock-data';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-pago-checkout',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './pago-checkout.html',
  styleUrls: ['./pago-checkout.css']
})
export class PagoCheckout implements OnInit, OnDestroy {
  private router = inject(Router);
  private comandaState = inject(ComandaState);
  private pagoService = inject(PagoService);
  private comandaHub = inject(ComandaHubService);
  private destroyRef = inject(DestroyRef);

  configuracion = configuracionRestauranteMock;
  estado = this.comandaState.estadoPedido;
  mesaId = this.comandaState.mesaId;
  cargandoPago = signal(false);
  pagoSolicitado = signal(false);
  error = signal<string | null>(null);

  constructor() {
    effect(() => {
      const modificada = this.comandaHub.comandaModificada();
      if (modificada) {
        if (modificada.estado === 'Finalizada') {
          this.router.navigate(['/comensal/pago-confirmado']);
        }
      }
    });
  }

  ngOnInit() {
    if (!this.estado()) {
      this.comandaState.consultarEstado();
    }
    const mesaId = this.comandaState.mesaId();
    if (mesaId) {
      this.comandaHub.conectarComoComensal(mesaId).catch(err =>
        console.error('Error al conectar hub de comanda:', err)
      );
    }
  }

  ngOnDestroy() {
    this.comandaHub.desconectarEscucha();
  }

  volver(): void {
    if (this.pagoSolicitado()) return;
    this.router.navigate(['/comensal/estado-pedido']);
  }

  pagarEfectivo(): void {
    const comandaId = this.estado()?.comandaId;
    if (!comandaId || this.pagoSolicitado()) return;

    this.cargandoPago.set(true);
    this.error.set(null);

    this.pagoService.solicitarPagoEfectivo(comandaId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.cargandoPago.set(false);
        this.pagoSolicitado.set(true);
      },
      error: (err) => {
        this.cargandoPago.set(false);
        this.error.set(err.error?.error || 'Error al solicitar el pago');
      }
    });
  }

  pagarElectronico(): void {
    this.error.set('Pago electrónico no disponible por el momento. Por favor abone en efectivo.');
  }
}
