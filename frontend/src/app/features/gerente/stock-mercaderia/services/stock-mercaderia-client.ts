import { inject, Injectable } from '@angular/core';
import { ApiClient } from '../../../../core/services/api-client';
import { ProductoStockMock } from '../../../../core/model/producto-stock-mock';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StockMercaderiaClient {
  private api = inject(ApiClient);
  private endpoint = '/stock-mercaderia';

  getStockMercaderia():Observable<ProductoStockMock[]> { 
    return this.api.get<ProductoStockMock[]>(`${this.endpoint}`) 
  }
  crear(producto: Partial<ProductoStockMock>): Observable<ProductoStockMock> {
    return this.api.post<ProductoStockMock>(this.endpoint, producto);
  }

  actualizar(id: number, producto: Partial<ProductoStockMock>): Observable<ProductoStockMock> {
    return this.api.put<ProductoStockMock>(`${this.endpoint}/${id}`, producto);
  }

  eliminar(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
