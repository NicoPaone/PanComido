import { computed, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from "jwt-decode";

import { ROLE_ROUTES, DEFAULT_ROUTE } from '../../app.constants';
import { ApiService } from './api-service';
import { Router } from '@angular/router';
import { JwtPayload } from '../models/domain/jwt-payload';
import { LoginResponse } from '../models/dtos/responses/login.response';
import { LoginRequest } from '../models/dtos/requests/login.request';

const TOKEN_KEY = 'pancomido_token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private api = inject(ApiService);
  private router = inject(Router);

  private rolActual = signal<string>('');
  private nombreActual = signal<string>('');
  private emailActual = signal<string>('');

  readonly esAutenticado = computed(() => !!this.rolActual());
  readonly rol = this.rolActual.asReadonly();
  readonly nombre = this.nombreActual.asReadonly();
  readonly email = this.emailActual.asReadonly();

  get restauranteId(): number {
    const payload = this.decodificarToken();
    return payload ? Number(payload.restauranteId) : 0;
  }
  get empleadoId(): number {
    const payload = this.decodificarToken();
    return payload ? Number(payload.sub) : 0;
  }

  constructor() {
    this.cargarSesion();
  }



  login(email: string, contrasenia: string): Observable<LoginResponse> {
    const body: LoginRequest = { email, contrasenia };
    return this.api.post<LoginResponse>('autenticacion/login', body).pipe(
      tap(response => {
        localStorage.setItem(TOKEN_KEY, response.token);
        this.rolActual.set(response.rol);
        this.nombreActual.set(response.nombre);
        this.emailActual.set(email);
      })
    )

  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this.rolActual.set('');
    this.nombreActual.set('');
    this.emailActual.set('');
    this.router.navigate(['/login']);
  }

  tieneRoles(roles: string[]): boolean {
    return roles.includes(this.rolActual());
  }

  obtenerRutaHome(): string {
    return ROLE_ROUTES[this.rolActual()] || DEFAULT_ROUTE;
  }

  obtenerPerfilUsuario() {
    return {
      name: this.nombreActual(),
      role: this.rolActual(),
      initials: this.generarIniciales(this.nombreActual()),
      avatarColor: '#f5a5a5'
    };
  }
  private cargarSesion(): void {
    const payload = this.decodificarToken();
    if (!payload) return;

    if (this.estaExpirado(payload)) {
      localStorage.removeItem(TOKEN_KEY);
      return;
    }
    this.rolActual.set(payload.role);
    this.nombreActual.set(payload.name);
    this.emailActual.set(payload.email);
  }
  private decodificarToken(): JwtPayload | null {

    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) return null;

    try {
      return jwtDecode<JwtPayload>(token);

    } catch {
      return null;
    }
  }
  private estaExpirado(payload: JwtPayload): boolean {
    if (!payload.exp) return false;
    return Date.now() >= payload.exp * 1000;
  }
  private generarIniciales(nombre: string): string {

    return nombre.split(' ')
      .map(parte => parte.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

}
