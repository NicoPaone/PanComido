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
   * POST /mesa/{mesaId}/ocupar
   * Body: { cantidadComensales: number }
   */
  ocuparMesa(mesaId: number, cantidadComensales: number): Observable<MesaOcuparResponse> {
    return this.api.post<MesaOcuparResponse>(
      `mesa/${mesaId}/ocupar`,
      { cantidadComensales }
    );
  }

  /**
   * Confirmar pedido con items del carrito
   */
  confirmarPedido(
    comandaId: number,
    request: ConfirmarPedidoRequest
  ): Observable<ComandaClienteResponse> {
    return this.api.post<ComandaClienteResponse>(
      `comanda/${comandaId}/cliente/confirmar-pedido`,
      request
    );
  }

  /**
   * Consultar estado actual del pedido
   */
  obtenerEstado(comandaId: number): Observable<ComandaClienteResponse> {
    return this.api.get<ComandaClienteResponse>(
      `comanda/${comandaId}/cliente/estado-pedido`
    );
  }
}
