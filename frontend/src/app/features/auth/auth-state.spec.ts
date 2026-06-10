import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthState } from './auth-state';
import { AuthService } from '../../core/services/auth.service';

describe('AuthState', () => {
  let state: AuthState;
  let authServiceMock: Pick<AuthService, keyof AuthService>;
  let routerMock: Pick<Router, keyof Router>;
  let loginMock: ReturnType<typeof vi.fn>;
  let logoutMock: ReturnType<typeof vi.fn>;
  let obtenerRutaHomeMock: ReturnType<typeof vi.fn>;
  let navigateMock: ReturnType<typeof vi.fn>;

  function configurarTest() {
    loginMock = vi.fn();
    logoutMock = vi.fn();
    obtenerRutaHomeMock = vi.fn();
    navigateMock = vi.fn();

    authServiceMock = {
      login: loginMock,
      logout: logoutMock,
      obtenerRutaHome: obtenerRutaHomeMock,
    } as unknown as Pick<AuthService, keyof AuthService>;

    routerMock = {
      navigate: navigateMock,
    } as unknown as Pick<Router, keyof Router>;

    TestBed.configureTestingModule({
      providers: [
        AuthState,
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock },
      ],
    });

    state = TestBed.inject(AuthState);
  }

  beforeEach(() => {
    configurarTest();
  });

  describe('login', () => {
    it('cuando el login es exitoso deberia navegar al home del rol', () => {
      loginMock.mockReturnValue(of({
        token: 'fake-token',
        rol: 'Gerente',
        nombre: 'Test',
      }));
      obtenerRutaHomeMock.mockReturnValue('staff/gerente');

      state.login('gerente@test.com', '1234');

      expect(loginMock).toHaveBeenCalledWith('gerente@test.com', '1234');
      expect(navigateMock).toHaveBeenCalledWith(['staff/gerente']);
      expect(state.cargando()).toBe(false);
      expect(state.error()).toBe('');
    });

    it('cuando el login falla deberia mostrar error y no navegar', () => {
      loginMock.mockReturnValue(throwError(() => new Error('error')));

      state.login('mal@test.com', 'wrong');

      expect(state.cargando()).toBe(false);
      expect(state.error()).toBe('Credenciales incorrectas');
      expect(navigateMock).not.toHaveBeenCalled();
    });

    it('deberia limpiar el error anterior antes de un nuevo login', () => {
      loginMock.mockReturnValue(throwError(() => new Error('error')));

      state.login('a@a.com', 'wrong');
      expect(state.error()).toBe('Credenciales incorrectas');

      loginMock.mockReturnValue(of({
        token: 't', rol: 'Mozo', nombre: 'N',
      }));
      obtenerRutaHomeMock.mockReturnValue('staff/mozo');

      state.login('b@b.com', 'ok');

      expect(state.error()).toBe('');
    });
  });

  describe('logout', () => {
    it('deberia delegar en AuthService.logout', () => {
      state.logout();

      expect(logoutMock).toHaveBeenCalledTimes(1);
    });
  });
});
