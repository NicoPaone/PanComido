import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../../../../core/services/api-client';
import { AuthService } from '../../../../core/services/auth.service';
import { NuevoProveedor, Proveedor } from '../../../../core/models/proveedor';

@Injectable({ providedIn: 'root' })
export class NuevoProveedorApiService {
  private api = inject(ApiClient);
  private authService = inject(AuthService);

  validateManagerCredentials(user: string, pass: string): Observable<boolean> {
    return this.authService.validateManagerCredentials(user, pass);
  }

  crearProveedor(proveedor: NuevoProveedor): Observable<Proveedor> {
    return this.api.post<Proveedor>('proveedores', proveedor);
  }
}
