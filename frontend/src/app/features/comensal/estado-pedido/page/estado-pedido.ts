import { Component, inject, OnInit, OnDestroy, effect } from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaStateService } from '../../services/comanda-state.service';
import { ComandaClienteResponse } from '../../../../core/models/comanda-cliente-response';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { ComandaHubService } from '../../../../core/services/hubs/comanda/comanda-hub-service';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
@Component({
  selector: 'app-estado-pedido',
  standalone: true,
  imports: [DecimalPipe, BotonComensal, LlamarAlMozo],
  templateUrl: './estado-pedido.html',
  styleUrls: ['./estado-pedido.css']
})
export class EstadoPedido implements OnInit, OnDestroy {
  private router = inject(Router);
  private comandaState = inject(ComandaStateService);
  private comandaHub = inject(ComandaHubService);

  configuracion = configuracionRestauranteMock;
  estado = this.comandaState.estadoPedido;
  mesaId = this.comandaState.mesaId;
  cargando = this.comandaState.cargando;
  error = this.comandaState.error;

  constructor() {
    effect(() => {
      const modificada = this.comandaHub.comandaModificada();
      if (modificada) {
        // Recargar el estado si hubo una notificación de SignalR
        this.comandaState.consultarEstado();
      }
    }, { allowSignalWrites: true });
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
    this.comandaHub.detener();
  }

  get estadoColor(): string {
    const st = this.estado()?.estadoUI?.toLowerCase() || '';
    if (st.includes('preparaci')) return '#ebd038'; // Amarillo
    if (st.includes('listo') || st.includes('hecho') || st.includes('espera')) return '#6bb446'; // Verde
    return '#a3a3a3'; // Gris (Recibido, default)
  }

  get estadoTextColor(): string {
    const st = this.estado()?.estadoUI?.toLowerCase() || '';
    if (st.includes('preparaci')) return '#000000'; // Texto negro para fondo amarillo
    return '#ffffff'; // Texto blanco para los demás
  }

  get estadoBorder(): string {
    return '#808080';
  }

  volver(): void {
    this.router.navigate(['/comensal/ver-carta']);
  }

  pagarCuenta(): void {
    this.router.navigate(['/comensal/pago-checkout']);
  }
}
