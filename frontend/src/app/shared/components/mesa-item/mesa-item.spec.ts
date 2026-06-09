import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MesaItem } from './mesa-item';
import { Mesa, EstadoMesa, FormaMesa } from '../../../core/models/domain/mesa';

describe('MesaItem', () => {
  let component: MesaItem;
  let fixture: ComponentFixture<MesaItem>;

  const mockMesa: Mesa = {
    id: 1,
    codigoInvitacion: 'abc123',
    cantidadPersonasMax: 4,
    numeroMesa: 1,
    posicionXInicio: 10,
    posicionXFin: 110,
    posicionYInicio: 10,
    posicionYFin: 110,
    dimensionMesa: { id: 1, forma: FormaMesa.Redonda },
    estadoMesa: EstadoMesa.Disponible,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MesaItem],
    }).compileComponents();

    fixture = TestBed.createComponent(MesaItem);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('mesa', mockMesa);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should compute ancho and alto from mesa dimensions', () => {
    expect(component.ancho()).toBe(100);
    expect(component.alto()).toBe(100);
  });

  it('should compute claseEstado based on estadoMesa', () => {
    expect(component.claseEstado()).toBe('estado-disponible');
  });

  it('should compute claseForma based on dimensionMesa', () => {
    expect(component.claseForma()).toBe(`forma-${FormaMesa.Redonda}`);
  });

  it('should emit clickMesa on manejarClickMesa', () => {
    const spy = vi.spyOn(component.clickMesa, 'emit');
    const event = new Event('click');
    const stopSpy = vi.spyOn(event, 'stopPropagation');
    component.manejarClickMesa(event);
    expect(stopSpy).toHaveBeenCalled();
    expect(spy).toHaveBeenCalledWith(1);
  });

  it('should emit cambioNumero on onInputBlur with valid number', () => {
    const spy = vi.spyOn(component.cambioNumero, 'emit');
    const event = { target: { value: '5' } } as any;
    component.onInputBlur(event);
    expect(spy).toHaveBeenCalledWith({ id: 1, numero: 5 });
  });

  it('should not emit cambioNumero on onInputBlur with invalid number', () => {
    const spy = vi.spyOn(component.cambioNumero, 'emit');
    const event = { target: { value: 'abc' } } as any;
    component.onInputBlur(event);
    expect(spy).not.toHaveBeenCalled();
  });
});
