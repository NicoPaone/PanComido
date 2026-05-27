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


import { PedidoService } from '../../../../../../app/core/services/pedido.service';


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
          provide: PedidoService,
          useValue: pedidoServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(
      CardPlatoComensalComponent
    );

    component = fixture.componentInstance;

    // INPUT MOCK
    component.plato = {
      nombre: 'Pizza',
      descripcion: 'Pizza muzzarella',
      precioVenta: 5000,
      imagen: 'imagen.jpg',
      tipo: 'principal'
    } as any;

    fixture.detectChanges();

  });

  // TESTS

  it('should create', () => {

    expect(component).toBeTruthy();

  });

  it('debería mostrar el nombre del plato', () => {

    expect(component.plato.nombre)
      .toBe('Pizza');

  });

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

  expect(spy).toHaveBeenCalledWith(
    component.plato
  );

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

    button.click();

    expect(spy).toHaveBeenCalled();

  });

});