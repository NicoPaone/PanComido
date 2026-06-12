import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';

import { DetallePedido } from './detalle-pedido';
import { PedidoState } from '../../services/pedido.state';
import { ComandaState } from '../../services/comanda-state';
import { ComensalState } from '../../services/comensal-state';

describe('DetallePedido', () => {
  let component: DetallePedido;
  let fixture: ComponentFixture<DetallePedido>;

  let routerMock: any;
  let pedidoStateMock: any;
  let comandaStateMock: any;
  let comensalStateMock: any;

  beforeEach(async () => {
    routerMock = {
      navigate: vi.fn()
    };

    pedidoStateMock = {
      pedidos: signal([
        {
          plato: {
            precioVentaFinal: 100
          },
          cantidad: 2
        }
      ])
    };

    comandaStateMock = {
      estadoPedido: signal({
        totalAPagar: 300
      }),
      tieneComandaActiva: vi.fn().mockReturnValue(true)
    };

    comensalStateMock = {};

    await TestBed.configureTestingModule({
      imports: [DetallePedido],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: PedidoState, useValue: pedidoStateMock },
        { provide: ComandaState, useValue: comandaStateMock },
        { provide: ComensalState, useValue: comensalStateMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DetallePedido);
    component = fixture.componentInstance;

    component.modal = {
      mostrar: vi.fn()
    } as any;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería calcular el total correctamente', () => {
    expect(component.total()).toBe(500);
  });

  it('debería volver al pedido', () => {
    component.volver();

    expect(routerMock.navigate)
      .toHaveBeenCalledWith(['/comensal/pedido']);
  });

  it('debería mostrar alerta si no hay comanda activa', () => {
    comandaStateMock.tieneComandaActiva.mockReturnValue(false);

    const alertSpy = vi
      .spyOn(window, 'alert')
      .mockImplementation(() => {});

    component.confirmarPedido();

    expect(alertSpy)
      .toHaveBeenCalledWith(
        'No hay mesa seleccionada. Por favor, escanea el QR de la mesa.'
      );

    expect(component.modal.mostrar)
      .not.toHaveBeenCalled();

    alertSpy.mockRestore();
  });

  it('debería mostrar alerta si el carrito está vacío', () => {
    pedidoStateMock.pedidos.set([]);

    const alertSpy = vi
      .spyOn(window, 'alert')
      .mockImplementation(() => {});

    component.confirmarPedido();

    expect(alertSpy)
      .toHaveBeenCalledWith('El carrito está vacío');

    expect(component.modal.mostrar)
      .not.toHaveBeenCalled();

    alertSpy.mockRestore();
  });

  it('debería mostrar el modal cuando todo es válido', () => {
    component.confirmarPedido();

    expect(component.modal.mostrar)
      .toHaveBeenCalled();
  });
});