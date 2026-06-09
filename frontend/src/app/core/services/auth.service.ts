import { Injectable, signal } from '@angular/core';
import { Observable, of, delay } from 'rxjs';


import { ROLE_ROUTES, DEFAULT_ROUTE } from '../../app.constants';
const ROLE_KEY = 'rol';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  //currentRole = signal<string>('Cocina');
  currentRole = signal<string>(sessionStorage.getItem(ROLE_KEY) || 'Gerente');

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
    sessionStorage.setItem(ROLE_KEY, role);
  }
  getHomeRoute(): string {
    const role = this.currentRole();
    return ROLE_ROUTES[role] || DEFAULT_ROUTE;
  }
}
