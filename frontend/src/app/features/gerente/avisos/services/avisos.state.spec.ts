import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { describe, expect, it, vi, beforeEach } from 'vitest';
import { Plato } from '../../../../core/models/plato';
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
      categoria: 'Principales',
      receta: [{ id: 1, nombre: 'Arroz', cantidad: 1, unidadMedida: 'KG' }]
    },
    {
      id: 2,
      nombre: 'Milanesa',
      precioVenta: 10000,
      costo: 5000,
      visible: true,
      imagen: '',
      categoria: 'Principales',
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

  it('debería mostrar platos sugeridos por cocina que no están visibles', () => {
    service.cargarSugerenciasCocina();

    expect(apiMock.getPlatos).toHaveBeenCalled();
    expect(service.sugerencias()).toHaveLength(1);
    expect(service.sugerencias()[0].nombre).toBe('Risotto a la crema');
  });

  it('debería hacer visible el plato al agregarlo a la carta', () => {
    service.cargarSugerenciasCocina();
    const sugerencia = service.sugerencias()[0];

    service.agregarSugerenciaACarta(sugerencia);

    expect(apiMock.updatePlato).toHaveBeenCalledWith(1, { visible: true });
    expect(service.sugerencias()).toHaveLength(0);
    expect(service.platoAgregadoACarta()?.nombre).toBe('Risotto a la crema');
  });

  it('debería cerrar la confirmación de plato agregado', () => {
    service.cargarSugerenciasCocina();
    service.agregarSugerenciaACarta(service.sugerencias()[0]);

    service.cerrarConfirmacionCarta();

    expect(service.platoAgregadoACarta()).toBeNull();
  });

  it('debería descartar la sugerencia al ignorarla', () => {
    service.cargarSugerenciasCocina();
    const sugerencia = service.sugerencias()[0];

    service.ignorarSugerencia(sugerencia);

    expect(service.sugerencias()).toHaveLength(0);
  });
});
