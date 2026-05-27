import { inject, Injectable } from '@angular/core';
import { ApiClient } from '../../../../core/services/api-client';
import { Observable } from 'rxjs';
import { ProductoStock } from '../../../../core/model/producto-stock';

@Injectable({
  providedIn: 'root',
})
export class StockMercaderiaClient {
  private api = inject(ApiClient);
  private endpoint = '/stock-mercaderia';

  getStockMercaderia():Observable<ProductoStock[]> { 
    return this.api.get<ProductoStock[]>(`${this.endpoint}`) 
  }
  crear(producto: Partial<ProductoStock>): Observable<ProductoStock> {
    return this.api.post<ProductoStock>(this.endpoint, producto);
  }

  actualizar(id: number, producto: Partial<ProductoStock>): Observable<ProductoStock> {
    return this.api.put<ProductoStock>(`${this.endpoint}/${id}`, producto);
  }

  eliminar(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
