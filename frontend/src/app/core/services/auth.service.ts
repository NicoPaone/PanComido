import { Injectable, signal } from '@angular/core';
import { Observable, of, delay } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Defaulting to 'Gerente' to keep the layout views functional during dev
  currentRole = signal<string>('Gerente');

  /**
   * Simulates manager credential validation.
   * NOTE: Replace this with a real backend API request in the future.
   */
  validateManagerCredentials(username: string, password: string): Observable<boolean> {
    // Mock check: gerente / 123456
    const isValid = username.toLowerCase().trim() === 'gerente' && password === '123456';
    return of(isValid).pipe(delay(250));
  }

  hasRole(roles: string[]): boolean {
    return roles.includes(this.currentRole());
  }

  setRole(role: string): void {
    this.currentRole.set(role);
  }
}
