import { DestroyRef, inject, Injectable, signal } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class AuthState {

  private authService = inject(AuthService);
  private router = inject(Router);;
  private destroyRef = inject(DestroyRef);


  readonly #cargando = signal(false);
  readonly #error = signal('');

  readonly cargando = this.#cargando.asReadonly();
  readonly error = this.#error.asReadonly();

  login(email: string, contrasenia: string): void{
    this.#error.set('');
    this.#cargando.set(true);

    this.authService.login(email,contrasenia)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: () => {
        this.#cargando.set(false);
        this.router.navigate([this.authService.obtenerRutaHome()]);
      },
      error: () => { 
        this.#cargando.set(false);
        this.#error.set('Credenciales incorrectas');

      }
    })
    
  }
  logout(): void {
    this.authService.logout();
  }

}
