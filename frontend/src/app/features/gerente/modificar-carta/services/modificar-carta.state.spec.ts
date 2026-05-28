import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ModificarCartaStateService } from './modificar-carta.state';
import { ModificarCartaApiService } from './modificar-carta.api';
import { Plato } from '../../../../core/models/plato';
import { vi } from 'vitest';

describe('ModificarCartaStateService', () => {
  let service: ModificarCartaStateService;
  let apiServiceMock: any;

  const mockPlatos: Plato[] = [
    { id: 1, nombre: 'Milanesa', precioVenta: 100, costo: 50, visible: true, receta: [], imagen: '' },
    { id: 2, nombre: 'Papas', precioVenta: 80, costo: 40, visible: false, receta: [], imagen: '' },
    { id: 3, nombre: 'Pizza', precioVenta: 120, costo: 60, visible: true, receta: [], imagen: '' }
  ];

  beforeEach(() => {
    apiServiceMock = {
      getPlatos: vi.fn().mockReturnValue(of([...mockPlatos])),
      updatePlato: vi.fn().mockImplementation((id, data) => {
        const found = mockPlatos.find(p => p.id === id);
        if (!found) throw new Error('Not found');
        return of({ ...found, ...data } as Plato);
      }),
      deletePlato: vi.fn().mockReturnValue(of(true))
    };

    TestBed.configureTestingModule({
      providers: [
        ModificarCartaStateService,
        { provide: ModificarCartaApiService, useValue: apiServiceMock }
      ]
    });

    service = TestBed.inject(ModificarCartaStateService);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('debería cargar platos al ejecutar cargarPlatos()', () => {
    expect(service.loading()).toBe(false);
    service.cargarPlatos();
    expect(apiServiceMock.getPlatos).toHaveBeenCalled();
    expect(service.platos()).toHaveLength(3);
    expect(service.loading()).toBe(false);
  });

  it('debería ordenar los platos por visibilidad (visibles primero) y filtrar por término de búsqueda', () => {
    service.cargarPlatos();
    // Orden natural por visibilidad: 1 (true), 3 (true), 2 (false)
    let filtered = service.filteredPlatos();
    expect(filtered[0].id).toBe(1);
    expect(filtered[1].id).toBe(3);
    expect(filtered[2].id).toBe(2);

    service.setSearchTerm('Papa');
    filtered = service.filteredPlatos();
    expect(filtered).toHaveLength(1);
    expect(filtered[0].nombre).toBe('Papas');
  });

  it('debería ocultar un plato visible tras esperar el tiempo de animación (450ms) y llamar al API', () => {
    vi.useFakeTimers();
    service.cargarPlatos();
    const plato = service.platos()[0]; // Milanesa (visible: true)

    service.toggleVisibility(plato);
    expect(service.explodingPlatoId()).toBe(1);

    vi.advanceTimersByTime(450);

    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(1, { visible: false });
    expect(service.platos().find(p => p.id === 1)?.visible).toBe(false);
    expect(service.explodingPlatoId()).toBeNull();
    vi.useRealTimers();
  });

  it('debería revertir la visibilidad a true si falla la petición API para ocultar el plato', () => {
    vi.useFakeTimers();
    apiServiceMock.updatePlato.mockReturnValueOnce(throwError(() => new Error('Error de red')));
    service.cargarPlatos();
    const plato = service.platos()[0];

    service.toggleVisibility(plato);
    vi.advanceTimersByTime(450);

    expect(service.platos().find(p => p.id === 1)?.visible).toBe(true);
    vi.useRealTimers();
  });

  it('debería mostrar un plato invisible inmediatamente sin esperar y llamar al API', () => {
    service.cargarPlatos();
    const plato = service.platos()[1]; // Papas (visible: false)

    service.toggleVisibility(plato);

    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(2, { visible: true });
    expect(service.platos().find(p => p.id === 2)?.visible).toBe(true);
    expect(service.explodingPlatoId()).toBeNull();
  });

  it('debería revertir la visibilidad a false si falla la petición API para mostrar el plato', () => {
    apiServiceMock.updatePlato.mockReturnValueOnce(throwError(() => new Error('Error de red')));
    service.cargarPlatos();
    const plato = service.platos()[1];

    service.toggleVisibility(plato);

    expect(service.platos().find(p => p.id === 2)?.visible).toBe(false);
  });

  it('debería abrir y cerrar los modales de edición y eliminación', () => {
    const plato = mockPlatos[0];
    service.setPlatoAEditar(plato);
    expect(service.platoAEditar()).toEqual(plato);

    service.setPlatoAEliminar(plato);
    expect(service.platoAEliminar()).toEqual(plato);

    service.closeModals();
    expect(service.platoAEditar()).toBeNull();
    expect(service.platoAEliminar()).toBeNull();
  });

  it('debería guardar cambios parciales de un plato editado al llamar a savePlato()', () => {
    service.cargarPlatos();
    const plato = service.platos()[0];
    service.setPlatoAEditar(plato);

    service.savePlato({ nombre: 'Milanesa Napolitana Suprema', precioVenta: 150 });

    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(1, {
      nombre: 'Milanesa Napolitana Suprema',
      precioVenta: 150
    });
    const updated = service.platos().find(p => p.id === 1);
    expect(updated?.nombre).toBe('Milanesa Napolitana Suprema');
    expect(updated?.precioVenta).toBe(150);
    expect(service.platoAEditar()).toBeNull();
  });

  it('debería eliminar un plato de la lista al confirmar eliminación con confirmDelete()', () => {
    service.cargarPlatos();
    const plato = service.platos()[2]; // Pizza
    service.setPlatoAEliminar(plato);

    service.confirmDelete();

    expect(apiServiceMock.deletePlato).toHaveBeenCalledWith(3);
    expect(service.platos()).toHaveLength(2);
    expect(service.platos().find(p => p.id === 3)).toBeUndefined();
    expect(service.platoAEliminar()).toBeNull();
  });
});
