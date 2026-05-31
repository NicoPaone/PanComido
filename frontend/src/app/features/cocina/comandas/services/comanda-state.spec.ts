import { TestBed } from '@angular/core/testing';
import { ComandaState } from './comanda-state';
import { ComandaService } from './comanda-service';
import { Comanda } from '../../../../core/models/comanda/comanda';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ComandaState', () => {
  let state: ComandaState;
  let mockService: any;

  const mockComandas: Comanda[] = [
    { id: 1, mesaId: 1, cantComensales: 2, estado: 'Nueva', estadoId: 1, horaInicio: '2026-05-30T10:00', horaFin: null, tiempoEstimadoTotal: 30, platos: [] },
    { id: 2, mesaId: 2, cantComensales: 4, estado: 'EnPreparacion', estadoId: 2, horaInicio: '2026-05-30T09:45', horaFin: null, tiempoEstimadoTotal: 45, platos: [] },
    { id: 3, mesaId: 3, cantComensales: 3, estado: 'EnEspera', estadoId: 3, horaInicio: '2026-05-30T10:20', horaFin: null, tiempoEstimadoTotal: 25, platos: [] }
  ];

  beforeEach(() => {
    mockService = {
      obtenerComandasActivas: vi.fn(),
      modificarEstadoComanda: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ComandaState,
        { provide: ComandaService, useValue: mockService }
      ]
    });

    state = TestBed.inject(ComandaState);
  });

  it('debería crearse correctamente', () => {
    expect(state).toBeTruthy();
  });

  describe('cargarComandasActivas()', () => {
    it('debería cargar y separar las comandas por estado', () => {
      mockService.obtenerComandasActivas.mockReturnValue(of(mockComandas));

      state.cargarComandasActivas();

      expect(state.comandasNuevas().length).toBe(1);
      expect(state.comandasEnPreparacion().length).toBe(1);
      expect(state.comandasEnEspera().length).toBe(1);
    });
  });

  describe('modificarEstadoComanda()', () => {
    it('debería actualizar el estado de una comanda', () => {
      const comandaActualizada: Comanda = { 
        ...mockComandas[0], 
        estado: 'EnPreparacion', 
        estadoId: 2 
      };

      mockService.modificarEstadoComanda.mockReturnValue(of(comandaActualizada));
      mockService.obtenerComandasActivas.mockReturnValue(of(mockComandas));

      state.cargarComandasActivas(); // precarga

      state.modificarEstadoComanda(1, 2);

      expect(mockService.modificarEstadoComanda).toHaveBeenCalledWith(1, 2);
    });
  });
});