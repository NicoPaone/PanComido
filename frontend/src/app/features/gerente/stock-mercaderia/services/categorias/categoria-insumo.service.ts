import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../../core/services/api-service';
import { CategoriaInsumo } from '../../../../../core/models/insumos/categorias/categoria-insumo';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})



export class CategoriaInsumoService {

  private api = inject(ApiService);

  private endpoint = 'categoria-insumo';

  obtenerCategorias(): Observable<CategoriaInsumo[]> {
    
    return this.api.get<CategoriaInsumo[]>(this.endpoint);
  }
}
