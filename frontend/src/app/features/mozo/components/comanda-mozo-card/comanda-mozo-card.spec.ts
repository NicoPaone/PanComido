import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComandaMozoCard } from './comanda-mozo-card';
import { Comanda } from '../../../../core/models/domain/comanda';

describe('ComandaMozoCard', () => {
  let component: ComandaMozoCard;
  let fixture: ComponentFixture<ComandaMozoCard>;

  const mockComanda: Comanda = {
    id: 1,
    mesaId: 1,
    cantComensales: 2,
    estado: 'Nueva',
    horaInicio: '2026-06-05T10:00:00',
    horaFin: null,
    horaUltimoCambioEstado: null,
    tiempoEstimadoTotal: 30,
    items: [],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaMozoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaMozoCard);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('comanda', mockComanda);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should compute headerClass based on estado', () => {
    expect(component.headerClass()).toBe('bg-nueva');
  });

  it('should emit ver output', () => {
    const spy = vi.spyOn(component.ver, 'emit');
    component.onVer();
    expect(spy).toHaveBeenCalledWith(1);
  });
});
