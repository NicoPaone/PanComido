import { Injectable, signal } from '@angular/core';
import { Observable, of, delay } from 'rxjs';

const ROLE_ROUTES: Record<string, string> = {
  'Gerente': 'staff/gerente',
  'Cocina':  'staff/cocina',
  'Mozo':    'staff/mozo',
};
const DEFAULT_ROUTE = 'staff/gerente';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentRole = signal<string>('Cocina');  

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