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

import { By } from '@angular/platform-browser';

import { ListaPlatosComensalComponent } from './lista-platos-comensal';

import { CardPlatoComensalComponent } from '../card-plato-comensal/card-plato-comensal';

describe('ListaPlatosComensalComponent', () => {

  let component: ListaPlatosComensalComponent;

  let fixture:
    ComponentFixture<ListaPlatosComensalComponent>;

  beforeEach(async () => {

    await TestBed.configureTestingModule({
      imports: [
        ListaPlatosComensalComponent
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(
      ListaPlatosComensalComponent
    );

    component = fixture.componentInstance;

    component.platos = [
      {
        nombre: 'Pizza',
        descripcion: 'Pizza muzza',
        precioVenta: 5000,
        imagen: 'pizza.jpg',
        tipo: 'principal'
      },
      {
        nombre: 'Hamburguesa',
        descripcion: 'Doble carne',
        precioVenta: 7000,
        imagen: 'burger.jpg',
        tipo: 'principal'
      }
    ] as any;

    fixture.detectChanges();

  });

  // TESTS

  it('should create', () => {

    expect(component).toBeTruthy();

  });

  it('debería renderizar cards de platos', () => {

    const cards =
      fixture.debugElement.queryAll(
        By.directive(
          CardPlatoComensalComponent
        )
      );

    expect(cards.length).toBe(2);

  });

  it('debería recibir los platos correctamente', () => {

    expect(component.platos.length)
      .toBe(2);

  });

  it('debería emitir agregarPedido', () => {

    const spy = vi.spyOn(
      component.agregarPedido,
      'emit'
    );

    const item = {
      plato: component.platos[0],
      cantidad: 1
    };

    component.onAgregarPedido(item);

    expect(spy).toHaveBeenCalledWith(
      item
    );

  });

});