import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ListaPlatosComensalComponent } from './lista-platos-comensal';
import { CartaItem } from '../../../../../core/models/domain/carta-item';

describe('ListaPlatosComensalComponent', () => {
  let component: ListaPlatosComensalComponent;
  let fixture: ComponentFixture<ListaPlatosComensalComponent>;

  const mockPlatos: CartaItem[] = [
    { articuloId: 1, nombre: 'Pizza', precioVentaFinal: 5000, urlImagen: 'pizza.jpg', costo: 2000, visibleEnCarta: true, tipoArticulo: 'Plato' },
    { articuloId: 2, nombre: 'Hamburguesa', precioVentaFinal: 7000, urlImagen: 'burger.jpg', costo: 3000, visibleEnCarta: true, tipoArticulo: 'Plato' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListaPlatosComensalComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ListaPlatosComensalComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('platos', mockPlatos);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('debería recibir los platos correctamente', () => {
    expect(component.platos().length).toBe(2);
  });

  it('debería emitir agregarPedido', () => {
    const spy = vi.spyOn(component.agregarPedido, 'emit');
    const item = { plato: mockPlatos[0], cantidad: 1 };
    component.onAgregarPedido(item);
    expect(spy).toHaveBeenCalledWith(item);
  });
});
