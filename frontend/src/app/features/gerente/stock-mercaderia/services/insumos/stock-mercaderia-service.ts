import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../../core/services/api-service';
import { Observable } from 'rxjs';
import { Insumo } from '../../../../../core/models/insumos/insumo';

@Injectable({
  providedIn: 'root',
})
export class StockMercaderiaService{
  private api = inject(ApiService);
  private endpoint = 'Insumo';


  getById(id:number): Observable<Insumo> {
    return this.api.get<Insumo>(`${this.endpoint}/${id}`);
  }

  getStockMercaderia():Observable<Insumo[]> { 
    return this.api.get<Insumo[]>(`${this.endpoint}`) 
  }

  crear(producto: Partial<Insumo>): Observable<Insumo> {
    return this.api.post<Insumo>(this.endpoint, producto);
  }

  actualizar(id: number, producto: Partial<Insumo>): Observable<Insumo> {
    return this.api.put<Insumo>(`${this.endpoint}/${id}`, producto);
  }

  eliminar(id: number): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
