import { Injectable, signal } from '@angular/core';
import { Observable, of, delay } from 'rxjs';


import { ROLE_ROUTES, DEFAULT_ROUTE } from '../../app.constants';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentRole = signal<string>('Gerente');

  // TODO: sacar ids harcodeados
  get currentRestauranteId(): number { return 1; }
  get currentMozoId(): number { return 3; }
  validateManagerCredentials(username: string, password: string): Observable<boolean> {
    const esValido = username.toLowerCase().trim() === 'gerente' && password === '123456';
    return of(esValido).pipe(delay(250));
  }

  hasRole(roles: string[]): boolean {
    return roles.includes(this.currentRole());
  }

  setRole(role: string): void {
    this.currentRole.set(role);
  }
  getHomeRoute(): string {
    const role = this.currentRole();
    return ROLE_ROUTES[role] || DEFAULT_ROUTE;
  }
}
