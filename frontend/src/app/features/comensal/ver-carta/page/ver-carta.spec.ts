import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { VerCarta } from './ver-carta';
import { PedidoState } from '../../services/pedido.state';
import { CartaState } from '../service/carta-state';
import { ComensalState } from '../../services/comensal-state';
import { signal } from '@angular/core';

describe('VerCartaComponent', () => {
  let component: VerCarta;
  let fixture: ComponentFixture<VerCarta>;

  let pedidoStateMock: any;
  let routerMock: any;
  let cartaStateMock: any;
  let comensalStateMock: any;

  beforeEach(async () => {
    pedidoStateMock = {
      agregarPedido: vi.fn(),
      cantidadTotal: signal(0),
      totalPrecio: signal(0)
    };

    routerMock = {
      navigate: vi.fn()
    };

    cartaStateMock = {
      cargarCarta: vi.fn(),
      setBusqueda: vi.fn(),
      setOrdenar: vi.fn(),
      itemsFiltrados: signal([]),
      cargando: signal(false),
      cantidadFiltrosActivos: signal(0),
      tieneFiltrosActivos: signal(false)
    };

    comensalStateMock = {
      enviando: signal(false), exito: signal(false), error: signal(null)
    };

    await TestBed.configureTestingModule({
      imports: [VerCarta],
      providers: [
        { provide: PedidoState, useValue: pedidoStateMock },
        { provide: Router, useValue: routerMock },
        { provide: CartaState, useValue: cartaStateMock },
        { provide: ComensalState, useValue: comensalStateMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(VerCarta);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería llamar a cargarCarta al iniciar', () => {
    component.ngOnInit();
    expect(cartaStateMock.cargarCarta).toHaveBeenCalled();
  });

  it('debería actualizar búsqueda en estado', () => {
    component.onSearch('pizza');
    expect(cartaStateMock.setBusqueda).toHaveBeenCalledWith('pizza');
  });

  it('debería seleccionar orden', () => {
    component.seleccionarOrden('precio-menor', 'Menor a mayor');
    expect(cartaStateMock.setOrdenar).toHaveBeenCalledWith('precio-menor');
    expect(component.ordenSeleccionado()).toBe('Menor a mayor');
    expect(component.menuOrdenarAbierto()).toBe(false);
  });

  it('debería agregar un item al pedido', () => {
    const item = { plato: { id: '1', nombre: 'Test', precioVentaFinal: 100 } as any, cantidad: 1 };
    component.agregarAlPedido(item);
    expect(pedidoStateMock.agregarPedido).toHaveBeenCalledWith(item);
  });

  it('debería navegar a pedido', () => {
    component.irAPedido();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/comensal/pedido']);
  });
});