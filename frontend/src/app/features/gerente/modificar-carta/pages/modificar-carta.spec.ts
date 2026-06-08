import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ModificarCartaComponent } from './modificar-carta';
import { ModificarCartaApiService } from '../services/modificar-carta.api';
import { Plato } from '../../../../core/models/domain/plato';
import { vi } from 'vitest';

describe('ModificarCartaComponent', () => {
  let component: ModificarCartaComponent;
  let fixture: ComponentFixture<ModificarCartaComponent>;
  let apiServiceMock: any;
  let routerMock: any;

  const mockPlatos: Plato[] = [
    { id: 1, nombre: 'Milanesa', precioVenta: 100, costo: 50, visible: true, receta: [], imagen: '' },
    { id: 2, nombre: 'Papas', precioVenta: 80, costo: 40, visible: false, receta: [], imagen: '' },
    { id: 3, nombre: 'Pizza', precioVenta: 120, costo: 60, visible: true, receta: [], imagen: '' }
  ];

  beforeEach(async () => {
    apiServiceMock = {
      getPlatos: vi.fn().mockReturnValue(of([...mockPlatos])),
      updatePlato: vi.fn().mockImplementation((id, data) => {
        const found = mockPlatos.find(p => p.id === id);
        return of({ ...found, ...data } as Plato);
      }),
      deletePlato: vi.fn().mockReturnValue(of(true))
    };

    routerMock = {
      navigate: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [ModificarCartaComponent],
      providers: [
        { provide: ModificarCartaApiService, useValue: apiServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ModificarCartaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar los platos al inicializar', () => {
    expect(apiServiceMock.getPlatos).toHaveBeenCalled();
    expect(component.platos()).toEqual(mockPlatos);
  });

  it('debería ordenar los platos visibles primero en filteredPlatos', () => {
    const filtered = component.filteredPlatos();
    expect(filtered[0].visible).toBe(true);
    expect(filtered[1].visible).toBe(true);
    expect(filtered[2].visible).toBe(false);
  });

  it('debería filtrar los platos por el término de búsqueda', () => {
    component.onSearch('mIla');
    const filtered = component.filteredPlatos();
    expect(filtered.length).toBe(1);
    expect(filtered[0].nombre).toBe('Milanesa');
  });

  it('debería manejar toggleVisibility para un plato visible con éxito tras el tiempo de espera', () => {
    vi.useFakeTimers();
    const plato = mockPlatos[0];
    component.toggleVisibility(plato);
    expect(component.explodingPlatoId()).toBe(plato.id);
    vi.advanceTimersByTime(450);
    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(plato.id, { visible: false });
    expect(component.platos().find(p => p.id === plato.id)?.visible).toBe(false);
    expect(component.explodingPlatoId()).toBeNull();
    vi.useRealTimers();
  });

  it('debería restaurar la visibilidad si falla la actualización de un plato visible tras el tiempo de espera', () => {
    vi.useFakeTimers();
    apiServiceMock.updatePlato.mockReturnValueOnce(throwError(() => new Error('Error')));
    const plato = mockPlatos[0];
    component.toggleVisibility(plato);
    vi.advanceTimersByTime(450);
    expect(component.platos().find(p => p.id === plato.id)?.visible).toBe(true);
    vi.useRealTimers();
  });

  it('debería manejar toggleVisibility para un plato invisible inmediatamente con éxito', () => {
    const plato = mockPlatos[1];
    component.toggleVisibility(plato);
    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(plato.id, { visible: true });
    expect(component.platos().find(p => p.id === plato.id)?.visible).toBe(true);
  });

  it('debería restaurar la visibilidad si falla la actualización de un plato invisible', () => {
    apiServiceMock.updatePlato.mockReturnValueOnce(throwError(() => new Error('Error')));
    const plato = mockPlatos[1];
    component.toggleVisibility(plato);
    expect(component.platos().find(p => p.id === plato.id)?.visible).toBe(false);
  });

  it('debería establecer el plato a editar al llamar a onEditPlato', () => {
    const plato = mockPlatos[0];
    component.onEditPlato(plato);
    expect(component.platoAEditar()).toEqual(plato);
  });

  it('debería establecer el plato a eliminar al llamar a onDeletePlato', () => {
    const plato = mockPlatos[0];
    component.onDeletePlato(plato);
    expect(component.platoAEliminar()).toEqual(plato);
  });

  it('debería guardar el plato editado al llamar a onSavePlato', () => {
    const plato = mockPlatos[0];
    component.onEditPlato(plato);
    component.onSavePlato({ nombre: 'Milanesa Editada' });
    expect(apiServiceMock.updatePlato).toHaveBeenCalledWith(plato.id, { nombre: 'Milanesa Editada' });
    expect(component.platos().find(p => p.id === plato.id)?.nombre).toBe('Milanesa Editada');
    expect(component.platoAEditar()).toBeNull();
  });

  it('debería no hacer nada en onSavePlato si no hay ningún plato editándose', () => {
    component.onSavePlato({ nombre: 'Milanesa Editada' });
    expect(apiServiceMock.updatePlato).not.toHaveBeenCalled();
  });

  it('debería eliminar el plato al llamar a onConfirmDelete', () => {
    const plato = mockPlatos[0];
    component.onDeletePlato(plato);
    component.onConfirmDelete();
    expect(apiServiceMock.deletePlato).toHaveBeenCalledWith(plato.id);
    expect(component.platos().find(p => p.id === plato.id)).toBeUndefined();
    expect(component.platoAEliminar()).toBeNull();
  });

  it('debería no hacer nada en onConfirmDelete si no hay ningún plato seleccionado para eliminar', () => {
    component.onConfirmDelete();
    expect(apiServiceMock.deletePlato).not.toHaveBeenCalled();
  });

  it('debería cerrar los modales al llamar a onCloseModals', () => {
    component.onEditPlato(mockPlatos[0]);
    component.onDeletePlato(mockPlatos[0]);
    component.onCloseModals();
    expect(component.platoAEditar()).toBeNull();
    expect(component.platoAEliminar()).toBeNull();
  });

  it('debería setear la categoría en el state al llamar a onCategoriaSeleccionada', () => {
    const spy = vi.spyOn(component['state'], 'setCategoria');
    component.onCategoriaSeleccionada('Bebidas');
    expect(spy).toHaveBeenCalledWith('Bebidas');
  });

  it('debería navegar a la vista de creación al llamar a irACrearPlato', () => {
    component.irACrearPlato();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff/gerente/crear-plato']);
  });

  it('debería alternar recomendado al llamar a toggleRecomendado', () => {
    const updateSpy = vi.spyOn(component['state'], 'toggleRecomendado');
    const plato = mockPlatos[0];
    component.toggleRecomendado(plato);
    expect(updateSpy).toHaveBeenCalledWith(plato);
  });
});
