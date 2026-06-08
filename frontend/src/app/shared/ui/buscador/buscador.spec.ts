import { ComponentFixture, TestBed } from '@angular/core/testing';
import { vi } from 'vitest';

import { Buscador } from './buscador';

describe('Buscador', () => {
  let component: Buscador;
  let fixture: ComponentFixture<Buscador>;

  beforeEach(async () => {
    // Suppress unhandled SignalR rejection from root-provided services leaking into test env
    const originalReject = window.onunhandledrejection;

    await TestBed.configureTestingModule({
      imports: [Buscador],
    }).compileComponents();

    fixture = TestBed.createComponent(Buscador);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit busquedaCambiada on input', () => {
    const spy = vi.spyOn(component.busquedaCambiada, 'emit');
    const inputEl = fixture.nativeElement.querySelector('input');
    inputEl.value = 'test';
    inputEl.dispatchEvent(new Event('input'));
    expect(spy).toHaveBeenCalledWith('test');
  });

  it('should clear valor and emit on limpiar()', () => {
    const spy = vi.spyOn(component.busquedaCambiada, 'emit');
    component.valor.set('something');
    component.limpiar();
    expect(component.valor()).toBe('');
    expect(spy).toHaveBeenCalledWith('');
  });
});
