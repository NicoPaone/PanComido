import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';

import { PersonalizarPlato } from './personalizar-plato';
import { PedidoState } from '../../services/pedido.state';
import { PlatoService } from '../../services/plato.service';
import { ComensalState } from '../../services/comensal-state';
import { ComandaState } from '../../services/comanda-state';

describe('PersonalizarPlato', () => {
  let component: PersonalizarPlato;
  let fixture: ComponentFixture<PersonalizarPlato>;

  let routerMock: any;
  let pedidoStateMock: any;
  let platoServiceMock: any;
  let comensalStateMock: any;
  let comandaStateMock: any;

  beforeEach(async () => {
    routerMock = {
      navigate: vi.fn()
    };

    pedidoStateMock = {
      actualizarObservaciones: vi.fn()
    };

    platoServiceMock = {
      getPlatoDetalle: vi.fn().mockReturnValue(
        of({
          ingredientes: [
            { nombre: 'Queso', opcional: true },
            { nombre: 'Tomate', opcional: true },
            { nombre: 'Lechuga', opcional: false }
          ]
        })
      )
    };

    comensalStateMock = {};
    comandaStateMock = {};

    await TestBed.configureTestingModule({
      imports: [PersonalizarPlato],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: PedidoState, useValue: pedidoStateMock },
        { provide: PlatoService, useValue: platoServiceMock },
        { provide: ComensalState, useValue: comensalStateMock },
        { provide: ComandaState, useValue: comandaStateMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PersonalizarPlato);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería volver al pedido', () => {
    component.volver();

    expect(routerMock.navigate)
      .toHaveBeenCalledWith(['/comensal/pedido']);
  });

  it('debería agregar un extra', () => {
    component.toggleExtra('Queso');

    expect(component.extrasSeleccionados)
      .toContain('Queso');
  });

  it('debería quitar un extra ya seleccionado', () => {
    component.extrasSeleccionados = ['Queso'];

    component.toggleExtra('Queso');

    expect(component.extrasSeleccionados)
      .not.toContain('Queso');
  });

  it('no debería agregar un extra si está en removidos', () => {
    component.removidosSeleccionados = ['Queso'];

    component.toggleExtra('Queso');

    expect(component.extrasSeleccionados)
      .not.toContain('Queso');
  });

  it('debería agregar un ingrediente a remover', () => {
    component.toggleRemover('Tomate');

    expect(component.removidosSeleccionados)
      .toContain('Tomate');
  });

  it('debería quitar un ingrediente removido', () => {
    component.removidosSeleccionados = ['Tomate'];

    component.toggleRemover('Tomate');

    expect(component.removidosSeleccionados)
      .not.toContain('Tomate');
  });

  it('no debería remover un ingrediente si está en extras', () => {
    component.extrasSeleccionados = ['Tomate'];

    component.toggleRemover('Tomate');

    expect(component.removidosSeleccionados)
      .not.toContain('Tomate');
  });

  it('debería guardar cambios', () => {
    component.itemIndex = 0;
    component.extrasSeleccionados = ['Queso'];
    component.removidosSeleccionados = ['Tomate'];
    component.observaciones = 'Sin sal';

    component.guardarCambios();

    expect(
      pedidoStateMock.actualizarObservaciones
    ).toHaveBeenCalledWith(
      0,
      '+ Queso, - Tomate',
      'Sin sal'
    );

    expect(routerMock.navigate)
      .toHaveBeenCalledWith(['/comensal/pedido']);
  });

  it('no debería guardar si itemIndex es -1', () => {
    component.itemIndex = -1;

    component.guardarCambios();

    expect(
      pedidoStateMock.actualizarObservaciones
    ).not.toHaveBeenCalled();
  });
});