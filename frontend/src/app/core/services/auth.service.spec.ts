import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { ApiService } from './api-service';

function crearTokenFake(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.firma_falsa`;
}

describe('AuthService', () => {
  let service: AuthService;
  let apiMock: Pick<ApiService, keyof ApiService>;
  let routerMock: Pick<Router, keyof Router>;
  let postMock: ReturnType<typeof vi.fn>;
  let navigateMock: ReturnType<typeof vi.fn>;

  function configurarTest() {
    postMock = vi.fn();
    navigateMock = vi.fn();

    apiMock = {
      post: postMock,
    } as unknown as Pick<ApiService, keyof ApiService>;

    routerMock = {
      navigate: navigateMock,
    } as unknown as Pick<Router, keyof Router>;

    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: ApiService, useValue: apiMock },
        { provide: Router, useValue: routerMock },
      ],
    });

    service = TestBed.inject(AuthService);
  }

  beforeEach(() => {
    configurarTest();
  });

  describe('login', () => {
    it('cuando el login es exitoso deberia guardar el token y actualizar signals', () => {
      const token = crearTokenFake({
        sub: '1',
        name: 'Gerente Test',
        email: 'gerente@test.com',
        role: 'Gerente',
        restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      });

      postMock.mockReturnValue(of({
        token,
        rol: 'Gerente',
        nombre: 'Gerente Test',
      }));

      service.login('gerente@test.com', '1234').subscribe();

      expect(localStorage.getItem('pancomido_token')).toBe(token);
      expect(service.rol()).toBe('Gerente');
      expect(service.nombre()).toBe('Gerente Test');
      expect(service.email()).toBe('gerente@test.com');
      expect(service.esAutenticado()).toBe(true);
    });
  });

  describe('logout', () => {
    it('deberia limpiar localStorage, resetear signals y navegar a login', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'Test', email: 'test@test.com',
        role: 'Mozo', restauranteId: '1',
      }));

      service['cargarSesion']();

      service.logout();

      expect(localStorage.getItem('pancomido_token')).toBeNull();
      expect(service.rol()).toBe('');
      expect(service.nombre()).toBe('');
      expect(service.email()).toBe('');
      expect(service.esAutenticado()).toBe(false);
      expect(navigateMock).toHaveBeenCalledWith(['/login']);
    });
  });

  describe('cargarSesion', () => {
    it('deberia restaurar la sesion desde localStorage si el token es valido', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '2', name: 'Cocina Test', email: 'cocina@test.com',
        role: 'Cocina', restauranteId: '2',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));

      service['cargarSesion']();

      expect(service.rol()).toBe('Cocina');
      expect(service.nombre()).toBe('Cocina Test');
      expect(service.email()).toBe('cocina@test.com');
      expect(service.esAutenticado()).toBe(true);
    });

    it('deberia limpiar el token si esta expirado', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '3', name: 'Expirado', email: 'exp@test.com',
        role: 'Mozo', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) - 3600,
      }));

      service['cargarSesion']();

      expect(localStorage.getItem('pancomido_token')).toBeNull();
      expect(service.esAutenticado()).toBe(false);
    });

    it('deberia no hacer nada si no hay token', () => {
      service['cargarSesion']();

      expect(service.esAutenticado()).toBe(false);
    });

    it('deberia no hacer nada si el token tiene formato invalido', () => {
      localStorage.setItem('pancomido_token', 'token-invalido-sin-puntos');

      service['cargarSesion']();

      expect(service.esAutenticado()).toBe(false);
    });
  });

  describe('tieneRoles', () => {
    it('deberia retornar true si el rol actual esta en la lista', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'Admin', email: 'a@a.com',
        role: 'Gerente', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.tieneRoles(['Gerente', 'Cocina'])).toBe(true);
    });

    it('deberia retornar false si el rol actual no esta en la lista', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'Mozo', email: 'm@m.com',
        role: 'Mozo', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.tieneRoles(['Gerente'])).toBe(false);
    });
  });

  describe('obtenerRutaHome', () => {
    it('deberia retornar staff/gerente para rol Gerente', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'G', email: 'g@test.com',
        role: 'Gerente', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.obtenerRutaHome()).toBe('staff/gerente');
    });

    it('deberia retornar staff/cocina para rol Cocina', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'C', email: 'c@test.com',
        role: 'Cocina', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.obtenerRutaHome()).toBe('staff/cocina');
    });

    it('deberia retornar staff/mozo para rol Mozo', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'M', email: 'm@test.com',
        role: 'Mozo', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.obtenerRutaHome()).toBe('staff/mozo');
    });

    it('deberia retornar ruta por defecto si el rol no esta mapeado', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '1', name: 'X', email: 'x@x.com',
        role: 'RolInexistente', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));
      service['cargarSesion']();

      expect(service.obtenerRutaHome()).toBe('staff/mozo');
    });
  });

  describe('restauranteId y empleadoId', () => {
    it('deberia extraer restauranteId del token', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '5', name: 'N', email: 'n@n.com',
        role: 'Gerente', restauranteId: '3',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));

      expect(service.restauranteId).toBe(3);
    });

    it('deberia extraer empleadoId del token', () => {
      localStorage.setItem('pancomido_token', crearTokenFake({
        sub: '42', name: 'N', email: 'n@n.com',
        role: 'Gerente', restauranteId: '1',
        exp: Math.floor(Date.now() / 1000) + 3600,
      }));

      expect(service.empleadoId).toBe(42);
    });

    it('deberia retornar 0 si no hay token', () => {
      localStorage.clear();

      expect(service.restauranteId).toBe(0);
      expect(service.empleadoId).toBe(0);
    });
  });
});
