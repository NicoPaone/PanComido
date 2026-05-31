import {
  describe,
  it,
  expect,
  beforeEach,
  vi
} from 'vitest';

import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';

import { Router } from '@angular/router';

import { VerCarta } from './ver-carta';

import { PedidoService } from '../../../../../app/core/services/pedido.service';

describe('VerCartaComponent', () => {

  let component: VerCarta;
  let fixture: ComponentFixture<VerCarta>;

  let pedidoServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {

    pedidoServiceMock = {
      agregarPedido: vi.fn()
    };

    routerMock = {
      navigate: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [VerCarta],
      providers: [
        {
          provide: PedidoService,
          useValue: pedidoServiceMock
        },
        {
          provide: Router,
          useValue: routerMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(VerCarta);

    component = fixture.componentInstance;

    fixture.detectChanges();

  });

  // TESTS

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería filtrar platos por búsqueda', () => {

    component.onSearch('pizza');

    expect(component.filteredPlatos.length).toBe(1);

    expect(
      component.filteredPlatos[0].nombre
    ).toContain('Pizza');

  });

  it('debería ordenar por menor precio', () => {

    component.ordenar('precio-menor');

    expect(
      component.filteredPlatos[0].precioVenta
    ).toBeLessThanOrEqual(
      component.filteredPlatos[1].precioVenta
    );

  });

  it('debería ordenar por mayor precio', () => {

    component.ordenar('precio-mayor');

    expect(
      component.filteredPlatos[0].precioVenta
    ).toBeGreaterThanOrEqual(
      component.filteredPlatos[1].precioVenta
    );

  });

  it('debería ordenar por nombre', () => {

    component.ordenar('nombre');

    expect(
      component.filteredPlatos[0].nombre
    ).toBe('Milanesa napolitana');

  });

  it('debería volver al orden predeterminado', () => {

    component.ordenar('precio-mayor');

    component.ordenar('default');

    expect(
      component.filteredPlatos.length
    ).toBe(component.platos.length);

  });

  it('debería filtrar platos principales', () => {

    component.tiposSeleccionados = ['principal'];

    component.aplicarFiltros();

    expect(
      component.filteredPlatos.every(
        plato => plato.tipo === 'principal'
      )
    ).toBe(true);

  });

  it('debería mostrar todos los platos sin filtros', () => {

    component.aplicarFiltros();

    expect(
      component.filteredPlatos.length
    ).toBe(component.platos.length);

  });

  it('debería agregar filtro al seleccionar checkbox', () => {

    const event = {
      target: {
        checked: true,
        value: 'principal'
      }
    } as any;

    component.toggleFiltro(
      event,
      component.tiposSeleccionados
    );

    expect(
      component.tiposSeleccionados
    ).toContain('principal');

  });

  it('debería agregar un item al pedido', () => {

    const item = {
      plato: component.platos[0],
      cantidad: 1
    };

    component.agregarAlPedido(item);

    expect(
      pedidoServiceMock.agregarPedido
    ).toHaveBeenCalledWith(item);

  });

  it('debería navegar a pedido', () => {

    component.irAPedido();

    expect(
      routerMock.navigate
    ).toHaveBeenCalledWith([
      '/comensal/pedido'
    ]);

  });

});