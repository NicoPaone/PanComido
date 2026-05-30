import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComandaPage } from './comanda-page';
import { ComandaState } from '../../services/comanda-state';
import { ComandoVozService } from '../../services/comando-voz/comando-voz.service';
import { signal } from '@angular/core';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ComandaPage', () => {
  let component: ComandaPage;
  let fixture: ComponentFixture<ComandaPage>;
  let mockState: any;
  let mockVozService: any;

  beforeEach(async () => {
    mockState = {
      cargarComandasActivas: vi.fn(),
      comandasEnPreparacion: signal([]),
      comandasNuevas: signal([]),
      comandasEnEspera: signal([]),
      modificarEstadoComanda: vi.fn()
    };

    mockVozService = {
      comandoDetectado: signal(null),
      enEscucha: signal(false),
      toggleListening: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ComandaPage],
      providers: [
        { provide: ComandaState, useValue: mockState },
        { provide: ComandoVozService, useValue: mockVozService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar las comandas activas al inicializar', () => {
    expect(mockState.cargarComandasActivas).toHaveBeenCalled();
  });

  it('debería reaccionar a comandos de voz y modificar el estado', () => {
    const comando = { comandaId: 10, nuevoEstadoId: 2 };
    mockVozService.comandoDetectado.set(comando);

    fixture.detectChanges();

    expect(mockState.modificarEstadoComanda).toHaveBeenCalledWith(10, 2);
  });

  it('debería llamar al toggleListening del servicio de voz', () => {
    component.vozService.toggleListening();
    expect(mockVozService.toggleListening).toHaveBeenCalled();
  });
});