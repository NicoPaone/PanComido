import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import { Plato } from '../../../../core/models/domain/plato';
import { AvisosApiService } from './avisos.api';
import { AvisosStateService } from './avisos.state';

describe('AvisosStateService', () => {
  let service: AvisosStateService;
  let apiMock: {
    getPlatos: ReturnType<typeof vi.fn>;
    updatePlato: ReturnType<typeof vi.fn>;
  };

  const platos: Plato[] = [
    {
      id: 1,
      nombre: 'Risotto a la crema',
      precioVenta: 12000,
      costo: 7000,
      visible: false,
      imagen: '',
      tipo: 'Principales',
      receta: [{ id: 1, nombre: 'Arroz', cantidad: 1, unidadMedida: 'KG' }]
    },
    {
      id: 2,
      nombre: 'Milanesa',
      precioVenta: 10000,
      costo: 5000,
      visible: true,
      imagen: '',
      tipo: 'Principales',
      receta: []
    }
  ];

  beforeEach(() => {
    apiMock = {
      getPlatos: vi.fn().mockReturnValue(of(platos)),
      updatePlato: vi.fn().mockImplementation((id: number, data: Partial<Plato>) =>
        of({ ...platos.find(plato => plato.id === id)!, ...data })
      )
    };

    TestBed.configureTestingModule({
      providers: [
        AvisosStateService,
        { provide: AvisosApiService, useValue: apiMock }
      ]
    });

    service = TestBed.inject(AvisosStateService);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });
});
