import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { map, Observable } from 'rxjs';
import { CartaItem } from '../../../../core/models/domain/carta-item';

@Injectable({
  providedIn: 'root',
})
export class CartaService {


  private api = inject(ApiService)
  private endpoint = 'carta'
  // obtenerCarta():Observable<CartaItem[]>{
  //   return this.api.get<CartaItem[]>(`${this.endpoint}/obtener-articulos`);
  // }
//   obtenerCarta(restauranteId: number): Observable<CartaItem[]> {
//   return this.api.get<CartaItem[]>(
//     `${this.endpoint}/${restauranteId}/comensal`
//   );
// }
private readonly restauranteId = 1;

// obtenerCarta(): Observable<CartaItem[]> {
//   return this.api.get<CartaItem[]>(
//     `carta/${this.restauranteId}/comensal`
//   );
// }

obtenerCarta(): Observable<CartaItem[]> {
  return this.api.get<any[]>(`carta/1/comensal`).pipe(
    map(data =>
      data.map(item => ({
        ...item,
        precioVentaFinal: item.precio
      }))
    )
  );
}

  
}
