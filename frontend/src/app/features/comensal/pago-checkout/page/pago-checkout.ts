import { Component, inject, signal, effect, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaStateService } from '../../services/comanda-state.service';
import { PagoService } from '../../../../core/services/pago.service';
import { ComandaHubService } from '../../../../core/services/hubs/comanda/comanda-hub-service';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';

@Component({
  selector: 'app-pago-checkout',
  standalone: true,
  imports: [DecimalPipe, BotonComensal],
  templateUrl: './pago-checkout.html',
  styleUrls: ['./pago-checkout.css']
})
export class PagoCheckout implements OnInit, OnDestroy {
  private router = inject(Router);
  private comandaState = inject(ComandaStateService);
  private pagoService = inject(PagoService);
  private comandaHub = inject(ComandaHubService);

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
        if (modificada.estado === 'Finalizada' || (modificada.estado as unknown as number) === 4) {
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
        console.error('Error conectando al hub:', err)
      );
    }
  }

  ngOnDestroy() {
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

    this.pagoService.solicitarPagoEfectivo(comandaId).subscribe({
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
