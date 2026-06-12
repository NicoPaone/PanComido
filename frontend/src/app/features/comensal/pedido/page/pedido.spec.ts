import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';

import { Pedido } from './pedido';
import { PedidoState } from '../../services/pedido.state';
import { ComensalState } from '../../services/comensal-state';
import { ComandaState } from '../../services/comanda-state';

describe('Pedido', () => {
  let component: Pedido;
  let fixture: ComponentFixture<Pedido>;

  let routerMock: any;
  let pedidoStateMock: any;
  let comensalStateMock: any;
  let comandaStateMock: any;

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
        },
        {
          plato: {
            precioVentaFinal: 50
          },
          cantidad: 1
        }
      ]),
      eliminarPedido: vi.fn(),
      incrementarCantidad: vi.fn(),
      decrementarCantidad: vi.fn()
    };

    comensalStateMock = {};
    comandaStateMock = {};

    await TestBed.configureTestingModule({
      imports: [Pedido],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: PedidoState, useValue: pedidoStateMock },
        { provide: ComensalState, useValue: comensalStateMock },
        { provide: ComandaState, useValue: comandaStateMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Pedido);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería calcular el total correctamente', () => {
    expect(component.total()).toBe(250);
  });

  it('debería navegar al detalle del pedido', () => {
    component.irADetallePedido();

    expect(routerMock.navigate)
      .toHaveBeenCalledWith(['/comensal/detalle-pedido']);
  });

  it('debería volver a la carta', () => {
    component.volver();

    expect(routerMock.navigate)
      .toHaveBeenCalledWith(['/comensal/ver-carta']);
  });

  it('debería eliminar un pedido', () => {
    component.eliminarPedido(1);

    expect(pedidoStateMock.eliminarPedido)
      .toHaveBeenCalledWith(1);
  });

  it('debería incrementar cantidad', () => {
    component.agregarAlPedido(1);

    expect(pedidoStateMock.incrementarCantidad)
      .toHaveBeenCalledWith(1);
  });

  it('debería decrementar cantidad', () => {
    component.eliminarUno(1);

    expect(pedidoStateMock.decrementarCantidad)
      .toHaveBeenCalledWith(1);
  });

  it('debería navegar a personalizar plato', () => {
    const item = {
      plato: {
        nombre: 'Pizza'
      },
      cantidad: 1
    } as any;

    component.irAPersonalizar(item, 2);

    expect(routerMock.navigate).toHaveBeenCalledWith(
      ['/comensal/personalizar-plato'],
      {
        state: {
          plato: item,
          index: 2
        }
      }
    );
  });
});