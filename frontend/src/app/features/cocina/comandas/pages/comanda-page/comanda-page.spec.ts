import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComandaPage } from './comanda-page';
import { ComandaState } from '../../services/comanda-state';
import { ComandoVozService } from '../../services/comando-voz/comando-voz.service';
import { ComandaHubService } from '../../../../../core/services/hubs/comanda/comanda-hub-service';
import { AuthService } from '../../../../../core/services/auth.service';
import { signal } from '@angular/core';
import { vi } from 'vitest';

describe('ComandaPage', () => {
  let component: ComandaPage;
  let fixture: ComponentFixture<ComandaPage>;
  let mockState: any;
  let mockVozService: any;
  let mockHub: any;
  let mockAuth: any;

  beforeEach(async () => {
    mockState = {
      cargarComandasActivas: vi.fn(),
      comandasEnPreparacion: signal([]),
      comandasNuevas: signal([]),
      comandasEnEspera: signal([]),
      comandas: signal([]),
      modificarEstadoComanda: vi.fn(),
      actualizarDesdeHub: vi.fn(),
    };

    mockVozService = {
      comandoDetectado: signal(null),
      enEscucha: signal(false),
      toggleListening: vi.fn()
    };

    mockHub = {
      comandaModificada: signal(null),
      conectarYUnirseGrupo: vi.fn().mockResolvedValue(undefined),
      desconectarEscucha: vi.fn(),
    };

    mockAuth = {
      currentRestauranteId: 1,
    };

    await TestBed.configureTestingModule({
      imports: [ComandaPage],
      providers: [
        { provide: ComandaState, useValue: mockState },
        { provide: ComandoVozService, useValue: mockVozService },
        { provide: ComandaHubService, useValue: mockHub },
        { provide: AuthService, useValue: mockAuth },
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

  it('debería llamar al toggleListening del servicio de voz', () => {
    component.vozService.toggleListening();
    expect(mockVozService.toggleListening).toHaveBeenCalled();
  });
});
