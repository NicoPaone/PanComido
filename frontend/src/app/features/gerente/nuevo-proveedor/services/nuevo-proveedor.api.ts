import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api-service';
import { AuthService } from '../../../../core/services/auth.service';
import { Proveedor } from '../../../../core/models/domain/proveedor';
import { NuevoProveedor } from '../../../../core/models/dtos/requests/proveedor.request';

@Injectable({ providedIn: 'root' })
export class NuevoProveedorApiService {
  private api = inject(ApiService);
  private authService = inject(AuthService);

  validateManagerCredentials(user: string, pass: string): Observable<boolean> {
    return this.authService.validateManagerCredentials(user, pass);
  }

  crearProveedor(proveedor: NuevoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>('proveedores', proveedor);
  }
}
