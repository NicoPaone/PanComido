import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal, WritableSignal } from '@angular/core';
import { Login } from './login';
import { AuthState } from '../../auth-state';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let cargandoSignal: WritableSignal<boolean>;
  let errorSignal: WritableSignal<string>;
  let loginMock: ReturnType<typeof vi.fn>;

  function crearAuthStateMock(): Partial<AuthState> {
    cargandoSignal = signal(false);
    errorSignal = signal('');

    return {
      cargando: cargandoSignal,
      error: errorSignal,
      login: loginMock,
    };
  }

  beforeEach(async () => {
    loginMock = vi.fn();

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [
        { provide: AuthState, useValue: crearAuthStateMock() },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deberia renderizar el formulario con email y contrasenia', () => {
    const form = fixture.nativeElement.querySelector('form');
    const emailInput = fixture.nativeElement.querySelector('#email');
    const passwordInput = fixture.nativeElement.querySelector('#contrasenia');
    const submitButton = fixture.nativeElement.querySelector('.login-btn');

    expect(form).toBeTruthy();
    expect(emailInput).toBeTruthy();
    expect(passwordInput).toBeTruthy();
    expect(submitButton).toBeTruthy();
    expect(submitButton.textContent).toContain('Ingresar');
  });

  it('deberia llamar a login con email y contrasenia al enviar el formulario', () => {
    const emailInput = fixture.nativeElement.querySelector('#email') as HTMLInputElement;
    const passwordInput = fixture.nativeElement.querySelector('#contrasenia') as HTMLInputElement;

    emailInput.value = 'test@test.com';
    emailInput.dispatchEvent(new Event('input'));
    passwordInput.value = '1234';
    passwordInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form')!;
    form.dispatchEvent(new Event('submit'));

    expect(loginMock).toHaveBeenCalledWith('test@test.com', '1234');
  });

  it('deberia mostrar el error cuando error tiene texto', () => {
    errorSignal.set('Credenciales incorrectas');
    fixture.detectChanges();

    const errorEl = fixture.nativeElement.querySelector('.login-error');
    expect(errorEl).toBeTruthy();
    expect(errorEl.textContent).toContain('Credenciales incorrectas');
  });

  it('deberia ocultar el error cuando esta vacio', () => {
    errorSignal.set('');
    fixture.detectChanges();

    const errorEl = fixture.nativeElement.querySelector('.login-error');
    expect(errorEl).toBeFalsy();
  });

  it('deberia deshabilitar el boton y mostrar spinner mientras carga', () => {
    cargandoSignal.set(true);
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('.login-btn') as HTMLButtonElement;
    expect(button.disabled).toBe(true);
    expect(button.textContent).toContain('Ingresando...');
  });

  it('deberia habilitar el boton cuando no esta cargando', () => {
    cargandoSignal.set(false);
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('.login-btn') as HTMLButtonElement;
    expect(button.disabled).toBe(false);
    expect(button.textContent).toContain('Ingresar');
  });
});
