import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ModificarCartaStateService } from './modificar-carta.state';
import { PlatoApiService } from '../../services/plato.api';
import { Plato } from '../../../../core/models/domain/plato';
import { vi } from 'vitest';

describe('ModificarCartaStateService', () => {
  let service: ModificarCartaStateService;
  let apiServiceMock: any;

  const mockPlatos: Plato[] = [
    { id: 1, nombre: 'Milanesa', precioVenta: 100, costo: 50, visible: true, recomendado: true, ventas: 10, receta: [], imagen: '', categoria: 'Principales' },
    { id: 2, nombre: 'Papas', precioVenta: 80, costo: 40, visible: false, recomendado: false, ventas: 30, receta: [], imagen: '', categoria: 'Entradas' },
    { id: 3, nombre: 'Pizza', precioVenta: 120, costo: 60, visible: true, recomendado: true, ventas: 20, receta: [], imagen: '', categoria: 'Principales' }
  ];

  beforeEach(() => {
    apiServiceMock = {
      getPlatos: vi.fn().mockReturnValue(of([...mockPlatos])),
      getPlatoById: vi.fn().mockImplementation((id) => {
        const found = mockPlatos.find(p => p.id === id);
        return of({ ...found, tipoPlatoId: 1, categoriaPlatoId: 1, tiempoPreparacion: 15, restriccionesIds: [] } as Plato);
      }),
      modificarPlato: vi.fn().mockImplementation((id, data) => {
        const found = mockPlatos.find(p => p.id === id);
        if (!found) throw new Error('Not found');
        return of({
          ...found,
          nombre: data.nombre,
          precioVenta: data.precioVentaFinal,
          visible: data.esVisibleEnCarta,
          imagen: data.urlImagen,
          receta: data.ingredientes.map((ingrediente: any) => ({
            id: ingrediente.insumoId,
            nombre: '',
            cantidad: ingrediente.cantidad,
            unidadMedida: ''
          }))
        } as Plato);
      }),
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
        { provide: PlatoApiService, useValue: apiServiceMock }
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
    expect(service.platoAEditar()).toEqual(expect.objectContaining(plato));

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

    expect(apiServiceMock.modificarPlato).toHaveBeenCalledWith(1, {
      nombre: 'Milanesa Napolitana Suprema',
      descripcion: '',
      precioVentaFinal: 150,
      tiempoPreparacionBase: 15,
      tipoPlatoId: 1,
      categoriaPlatoId: 1,
      urlImagen: '',
      esVisibleEnCarta: true,
      restriccionesIds: [],
      ingredientes: []
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

  it('debería filtrar y ordenar platosRecomendados por ventas de forma descendente', () => {
    service.cargarPlatos();
    const recomendados = service.platosRecomendados();
    expect(recomendados).toHaveLength(2);
    // Pizza (id: 3, ventas: 20) debe estar primero que Milanesa (id: 1, ventas: 10)
    expect(recomendados[0].id).toBe(3);
    expect(recomendados[1].id).toBe(1);
  });

  it('debería filtrar platosNormales excluyendo los recomendados visibles', () => {
    service.cargarPlatos();
    const normales = service.platosNormales();
    expect(normales).toHaveLength(1);
    expect(normales[0].id).toBe(2); // Papas
  });

  it('debería excluir platos recomendados invisibles de platosRecomendados y enviarlos al final de platosNormales', () => {
    service.cargarPlatos();
    
    expect(service.platosRecomendados()).toHaveLength(2);
    expect(service.platosNormales()).toHaveLength(1);

    const pizza = service.platos().find(p => p.id === 3)!;
    
    vi.useFakeTimers();
    service.toggleVisibility(pizza);
    vi.advanceTimersByTime(450);
    vi.useRealTimers();

    const recomendados = service.platosRecomendados();
    expect(recomendados).toHaveLength(1);
    expect(recomendados[0].id).toBe(1);

    const normales = service.platosNormales();
    expect(normales).toHaveLength(2);
    expect(normales[0].id).toBe(2); // Papas (invisible, normal)
    expect(normales[1].id).toBe(3); // Pizza (invisible, recomendado)
  });

  it('debería alternar el estado recomendado tras llamar a toggleRecomendado()', () => {
    service.cargarPlatos();
    const platoNormal = service.platos()[1]; // Papas (recomendado: false)

    service.toggleRecomendado(platoNormal);

    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(2, { recomendado: true });
    expect(service.platos().find(p => p.id === 2)?.recomendado).toBe(true);
  });

  it('debería revertir el estado recomendado si falla la petición API al alternar recomendación', () => {
    apiServiceMock.updatePlato.mockReturnValueOnce(throwError(() => new Error('Error de red')));
    service.cargarPlatos();
    const platoNormal = service.platos()[1]; // Papas (recomendado: false)

    service.toggleRecomendado(platoNormal);

    expect(service.platos().find(p => p.id === 2)?.recomendado).toBe(false);
  });

  it('debería filtrar platos por categoría al llamar a setCategoria()', () => {
    service.cargarPlatos();
    
    // Al filtrar por 'Principales'
    service.setCategoria('Principales');
    expect(service.selectedCategoria()).toBe('Principales');
    
    // Milanesa (id: 1) y Pizza (id: 3) son Principales recomendados
    const recomendados = service.platosRecomendados();
    expect(recomendados).toHaveLength(2);
    expect(recomendados[0].id).toBe(3); // Pizza (ventas: 20)
    expect(recomendados[1].id).toBe(1); // Milanesa (ventas: 10)
    
    // Papas (id: 2, Entradas) no debe aparecer en platosNormales
    expect(service.platosNormales()).toHaveLength(0);
    
    // Al filtrar por 'Entradas'
    service.setCategoria('Entradas');
    expect(service.selectedCategoria()).toBe('Entradas');
    expect(service.platosRecomendados()).toHaveLength(0); // Ninguno es Entrada
    expect(service.platosNormales()).toHaveLength(1);
    expect(service.platosNormales()[0].id).toBe(2); // Papas
  });

  it('debería restablecer el filtro de categoría al pasar null a setCategoria()', () => {
    service.cargarPlatos();
    
    service.setCategoria('Principales');
    expect(service.platosNormales()).toHaveLength(0);
    
    service.setCategoria(null);
    expect(service.selectedCategoria()).toBeNull();
    expect(service.platosRecomendados()).toHaveLength(2);
    expect(service.platosNormales()).toHaveLength(1);
  });
});
