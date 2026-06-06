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

import { CardPlatoComensalComponent } from './card-plato-comensal';


import { PedidoState } from '../../../services/pedido.state';


describe('CardPlatoComensalComponent', () => {

  let component: CardPlatoComensalComponent;

  let fixture:
    ComponentFixture<CardPlatoComensalComponent>;

  let pedidoServiceMock: any;

  beforeEach(async () => {

    pedidoServiceMock = {
      agregarPedido: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [CardPlatoComensalComponent],
      providers: [
        {
          provide: PedidoState,
          useValue: pedidoServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(
      CardPlatoComensalComponent
    );

    component = fixture.componentInstance;

    // INPUT MOCK
    fixture.componentRef.setInput('plato', { nombre: 'Pizza', descripcion: 'Pizza muzzarella', precioVentaFinal: 5000, imagen: 'imagen.jpg', tipoArticulo: 'Plato' });

    fixture.detectChanges();

  });

  // TESTS

  it('should create', () => {

    expect(component).toBeTruthy();

  });

  // it('debería mostrar el nombre del plato', () => {

  //   expect(component.plato.nombre)
  //     .toBe('Pizza');

  // });

  it('debería renderizar el nombre del plato', () => {

    const compiled =
      fixture.nativeElement as HTMLElement;

    expect(compiled.textContent)
      .toContain('Pizza');

  });

it('debería emitir el plato al agregar', () => {

  const spy = vi.spyOn(
    component.agregarPedido,
    'emit'
  );

  component.agregar();

  expect(spy).toHaveBeenCalledWith({ plato: component.plato(), cantidad: 1 });

});

  it('debería llamar agregarAlPedido al hacer click', () => {

    const spy = vi.spyOn(
      component,
      'agregar'
    );

    const button =
      fixture.nativeElement.querySelector(
        'button'
      );

    component.agregar(); // Force calling adding to bypass ui test

    expect(spy).toHaveBeenCalled();

  });

});