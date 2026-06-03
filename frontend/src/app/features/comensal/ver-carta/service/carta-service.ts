import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { Observable } from 'rxjs';
import { CartaItem } from '../../../../core/models/carta-item';

@Injectable({
  providedIn: 'root',
})
export class CartaService {

  private api = inject(ApiService);

<<<<<<< HEAD
obtenerCarta(): Observable<CartaItem[]> {
  return this.api.get<CartaItem[]>('carta/obtener-articulos');
=======
  private api = inject(ApiService)
  private endpoint = 'carta'
  obtenerCarta():Observable<CartaItem[]>{
    return this.api.get<CartaItem[]>(`${this.endpoint}/obtener-articulos`);
  }

  
>>>>>>> 8ce2bf40f4a5cad7311dfe784aa30df3c8dad8f5
}
}