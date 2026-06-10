import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MapaMesas } from './mapa-mesas';
import { MesaState } from '../../services/mesa.state';
import { AuthService } from '../../../../core/services/auth.service';
import { signal } from '@angular/core';
import { vi } from 'vitest';

describe('MapaMesas', () => {
  let component: MapaMesas;
  let fixture: ComponentFixture<MapaMesas>;
  let mockState: any;
  let mockAuth: any;

  beforeEach(async () => {
    mockState = {
      cargarMesas: vi.fn(),
      mesas: signal([]),
      mesaSeleccionada: signal(null),
      notificacion: signal(null),
      seleccionarMesa: vi.fn(),
      ocuparMesa: vi.fn(),
      isEditorMode: signal(false),
    };

    mockAuth = {
      currentRestauranteId: 1,
      hasRole: vi.fn().mockReturnValue(true),
    };

    await TestBed.configureTestingModule({
      imports: [MapaMesas],
      providers: [
        { provide: MesaState, useValue: mockState },
        { provide: AuthService, useValue: mockAuth },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MapaMesas);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call cargarMesas on init', () => {
    expect(mockState.cargarMesas).toHaveBeenCalled();
  });

  it('should select and deselect mobile mesa', () => {
    const mesa = { id: 1, numeroMesa: 1 } as any;
    component.seleccionarMesaMobile(mesa);
    expect(component.mesaMobileSeleccionada()).toEqual(mesa);
    component.volverGridMobile();
    expect(component.mesaMobileSeleccionada()).toBeNull();
  });
});
