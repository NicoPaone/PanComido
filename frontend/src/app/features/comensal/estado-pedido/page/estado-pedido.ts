import { Component, inject, OnInit, OnDestroy, effect , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { ComandaState } from '../../services/comanda-state';
import { ComandaClienteResponse } from '../../../../core/models/dtos/responses/comanda-cliente.response';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';
import { ComandaHubService } from '../../../../core/services/hubs/comanda/comanda-hub-service';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { ComensalState } from '../../services/comensal-state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-estado-pedido',
  standalone: true,
  imports: [DecimalPipe, BotonComensal, LlamarAlMozo],
  templateUrl: './estado-pedido.html',
  styleUrls: ['./estado-pedido.css']
})
export class EstadoPedido implements OnInit, OnDestroy {
  private router = inject(Router);
  private comandaState = inject(ComandaState);
  private comandaHub = inject(ComandaHubService);
  comensalState = inject(ComensalState);

  configuracion = configuracionRestauranteMock;
  estado = this.comandaState.estadoPedido;
  mesaId = this.comandaState.mesaId;
  cargando = this.comandaState.cargando;
  error = this.comandaState.error;

  constructor() {
    effect(() => {
      const modificada = this.comandaHub.comandaModificada();
      if (modificada) {
        // recargo si llega un push de signalr
        this.comandaState.consultarEstado();
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
        void 0
      );
    }
  }

  ngOnDestroy() {
    this.comandaHub.desconectarEscucha();
  }

  get estadoColor(): string {
    const st = this.estado()?.estadoUI?.toLowerCase() || '';
    if (st.includes('preparaci')) return '#ebd038'; // amarillo
    if (st.includes('listo') || st.includes('hecho') || st.includes('espera')) return '#6bb446'; // verde
    return '#a3a3a3'; // gris por defecto
  }

  get estadoTextColor(): string {
    const st = this.estado()?.estadoUI?.toLowerCase() || '';
    if (st.includes('preparaci')) return '#000000'; 
    return '#ffffff';
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
