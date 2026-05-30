import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BotonVoz } from './boton-voz';
import { describe, it, expect, beforeEach } from 'vitest';

describe('BotonVoz', () => {
  let component: BotonVoz;
  let fixture: ComponentFixture<BotonVoz>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BotonVoz]
    }).compileComponents();

    fixture = TestBed.createComponent(BotonVoz);
    component = fixture.componentInstance;

    fixture.componentRef.setInput('enEscucha', false);

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería mostrar el ícono de micrófono cuando NO está escuchando', () => {
    fixture.componentRef.setInput('enEscucha', false);
    fixture.detectChanges();

    const svg = fixture.nativeElement.querySelector('svg');
    expect(svg).toBeTruthy();
    expect(svg.classList.contains('icon-mic')).toBe(true);
  });

  it('debería mostrar las ondas de audio cuando está escuchando', () => {
    fixture.componentRef.setInput('enEscucha', true);
    fixture.detectChanges();

    const wavesContainer = fixture.nativeElement.querySelector('.audio-waves');
    expect(wavesContainer).toBeTruthy();

    const waves = fixture.nativeElement.querySelectorAll('.wave');
    expect(waves.length).toBe(4);
  });

  it('debería tener la clase "is-listening" cuando enEscucha = true', () => {
    fixture.componentRef.setInput('enEscucha', true);
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('button');
    expect(button.classList.contains('is-listening')).toBe(true);
  });

  it('debería emitir el evento toggle al hacer click', () => {
    let emitted = false;
    component.toggle.subscribe(() => emitted = true);

    const button = fixture.nativeElement.querySelector('button');
    button.click();

    expect(emitted).toBe(true);
  });

  it('debería cambiar el title según el estado', () => {
    // Estado inactivo
    fixture.componentRef.setInput('enEscucha', false);
    fixture.detectChanges();
    let button = fixture.nativeElement.querySelector('button');
    expect(button.getAttribute('title')).toBe('Activar comandos de voz');

    // Estado escuchando
    fixture.componentRef.setInput('enEscucha', true);
    fixture.detectChanges();
    button = fixture.nativeElement.querySelector('button');
    expect(button.getAttribute('title')).toBe('Apagar micrófono');
  });
});