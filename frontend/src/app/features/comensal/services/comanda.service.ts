import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api-service';
import { MesaOcuparResponse } from '../../../core/models/dtos/responses/mesa-ocupar.response';
import { ComandaClienteResponse } from '../../../core/models/dtos/responses/comanda-cliente.response';
import { ConfirmarPedidoRequest } from '../../../core/models/dtos/requests/confirmar-pedido.request';

@Injectable({ providedIn: 'root' })
export class ComandaService {
  private api = inject(ApiService);

  /**
   * POST /mesa/comensal/{restauranteId}/{mesaId}/ocupar
   * Body: { cantidadComensales: number }
   */
  ocuparMesa(restauranteId: number, mesaId: number, cantidadComensales: number): Observable<MesaOcuparResponse> {
    return this.api.post<MesaOcuparResponse>(
      `mesa/comensal/${restauranteId}/${mesaId}/ocupar`,
      { cantidadComensales }
    );
  }

  /**
   * Confirmar pedido con items del carrito
   * POST /comanda/{comandaId}/comensal/{restauranteId}/confirmar-pedido
   */
  confirmarPedido(
    comandaId: number,
    restauranteId: number,
    request: ConfirmarPedidoRequest
  ): Observable<ComandaClienteResponse> {
    return this.api.post<ComandaClienteResponse>(
      `comanda/${comandaId}/comensal/${restauranteId}/confirmar-pedido`,
      request
    );
  }

  /**
   * Consultar estado actual del pedido
   * GET /comanda/{comandaId}/comensal/{restauranteId}/estado-pedido
   */
  obtenerEstado(comandaId: number, restauranteId: number): Observable<ComandaClienteResponse> {
    return this.api.get<ComandaClienteResponse>(
      `comanda/${comandaId}/comensal/${restauranteId}/estado-pedido`
    );
  }
}
