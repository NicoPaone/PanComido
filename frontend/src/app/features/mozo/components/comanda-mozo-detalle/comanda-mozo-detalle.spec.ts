import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComandaMozoDetalle } from './comanda-mozo-detalle';
import { Comanda } from '../../../../core/models/domain/comanda';

describe('ComandaMozoDetalle', () => {
  let component: ComandaMozoDetalle;
  let fixture: ComponentFixture<ComandaMozoDetalle>;

  const mockComanda: Comanda = {
    id: 1,
    mesaId: 1,
    cantComensales: 2,
    estado: 'EnPreparacion',
    horaInicio: '2026-06-05T10:00:00',
    horaFin: null,
    horaUltimoCambioEstado: null,
    tiempoEstimadoTotal: 30,
    items: [
      { id: 1, entregado: false, cantidad: 1, observacionesGenerales: null, observacionesIngredientes: null, articulo: { id: 1, nombre: 'Milanesa', urlImagen: null } },
      { id: 2, entregado: true, cantidad: 2, observacionesGenerales: null, observacionesIngredientes: null, articulo: { id: 2, nombre: 'Papas', urlImagen: null } },
    ],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaMozoDetalle],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaMozoDetalle);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('comanda', mockComanda);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter only non-delivered items', () => {
    expect(component.itemsNoEntregados().length).toBe(1);
    expect(component.itemsNoEntregados()[0].id).toBe(1);
  });

  it('should toggle item selection', () => {
    component.toggleItem(1);
    expect(component.isSelected(1)).toBe(true);
    expect(component.haySeleccion()).toBe(true);
    component.toggleItem(1);
    expect(component.isSelected(1)).toBe(false);
  });

  it('should toggle all items', () => {
    component.toggleTodos();
    expect(component.todosSeleccionados()).toBe(true);
    component.toggleTodos();
    expect(component.seleccionados().size).toBe(0);
  });

  it('should emit entregar on aplicar', () => {
    const spy = vi.spyOn(component.entregar, 'emit');
    component.toggleItem(1);
    component.aplicar();
    expect(spy).toHaveBeenCalledWith({ comandaId: 1, articuloComandaIds: [1] });
    expect(component.seleccionados().size).toBe(0);
  });

  it('should emit cerrar on onCerrar', () => {
    const spy = vi.spyOn(component.cerrar, 'emit');
    component.onCerrar();
    expect(spy).toHaveBeenCalled();
  });
});
