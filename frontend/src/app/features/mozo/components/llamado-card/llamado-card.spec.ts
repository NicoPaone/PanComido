import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LlamadoCard } from './llamado-card';
import { Llamado } from '../../../../core/models/domain/llamado';

describe('LlamadoCard', () => {
  let component: LlamadoCard;
  let fixture: ComponentFixture<LlamadoCard>;

  const mockLlamado: Llamado = {
    id: 1,
    mozoId: 1,
    mesaId: 3,
    gerenteId: null,
    categoriaLlamadoId: 1,
    categoriaDescripcion: 'Cubiertos',
    descripcion: 'Faltan tenedores',
    resuelto: false,
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LlamadoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(LlamadoCard);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('llamado', mockLlamado);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display mesa title', () => {
    expect(component.titulo()).toBe('Mesa 3');
  });

  it('should resolve resolver output on emit', () => {
    const spy = vi.spyOn(component.resolver, 'emit');
    component.resolver.emit(1);
    expect(spy).toHaveBeenCalledWith(1);
  });
});
