import { TestBed } from '@angular/core/testing';
import { ComandaService } from './comanda-service';
import { ApiService } from '../../../../core/services/api-service';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ComandaService', () => {
  let service: ComandaService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      get: vi.fn(),
      put: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ComandaService,
        { provide: ApiService, useValue: mockApi }
      ]
    });

    service = TestBed.inject(ComandaService);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('obtenerComandasActivas() debería llamar al endpoint correcto', () => {
    mockApi.get.mockReturnValue(of([]));

    service.obtenerComandasActivas().subscribe();

    expect(mockApi.get).toHaveBeenCalledWith('Comanda/activas');
  });

  it('modificarEstadoComanda() debería llamar con los parámetros correctos', () => {
    mockApi.put.mockReturnValue(of({}));

    service.modificarEstadoComanda(5, 2).subscribe();

    expect(mockApi.put).toHaveBeenCalledWith('Comanda/activas/5/2');
  });
});