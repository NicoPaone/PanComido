import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaStateService } from '../../services/comanda-state.service';
import { ComandaClienteResponse } from '../../../../core/models/comanda-cliente-response';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';

@Component({
  selector: 'app-estado-pedido',
  standalone: true,
  imports: [DecimalPipe, BotonComensal],
  templateUrl: './estado-pedido.html',
  styleUrls: ['./estado-pedido.css']
})
export class EstadoPedido implements OnInit, OnDestroy {
  private router = inject(Router);
  private comandaState = inject(ComandaStateService);

  configuracion = configuracionRestauranteMock;
  estado: ComandaClienteResponse | null = null;
  cargando = this.comandaState.cargando;
  error = this.comandaState.error;
  private intervalId: ReturnType<typeof setInterval> | null = null;

  ngOnInit() {
    this.cargarEstado();
    // Poll cada 10 segundos
    this.intervalId = setInterval(() => this.cargarEstado(), 10000);
  }

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
  }

  async cargarEstado() {
    try {
      this.estado = await this.comandaState.consultarEstado();
    } catch (e) {
      console.error('Error consultando estado', e);
    }
  }

  get estadoLabel(): string {
    switch (this.estado?.estadoUI) {
      case 'Recibido': return '⏳ Recibido';
      case 'Preparación': return '👨‍🍳 En preparación';
      case 'Listo': return '✅ Listo para servir';
      case 'Esperando pedido': return '📝 Esperando pedido';
      default: return 'Esperando...';
    }
  }

  get estadoColor(): string {
    switch (this.estado?.estadoUI) {
      case 'Recibido': return '#f59e0b';
      case 'Preparación': return '#3b82f6';
      case 'Listo': return '#10b981';
      case 'Esperando pedido': return '#6b7280';
      default: return '#9ca3af';
    }
  }

  volver(): void {
    this.router.navigate(['/comensal/ver-carta']);
  }
}
