import { ComponentFixture, TestBed } from '@angular/core/testing';
import { KdsContadorTiempo } from './kds-contador-tiempo';

describe('KdsContadorTiempo', () => {
  let component: KdsContadorTiempo;
  let fixture: ComponentFixture<KdsContadorTiempo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KdsContadorTiempo],
    }).compileComponents();

    fixture = TestBed.createComponent(KdsContadorTiempo);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('fechaInicio', new Date(Date.now() - 65000).toISOString());
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should compute formatted time from fechaInicio', () => {
    const result = component.tiempoFormateado();
    expect(result).toMatch(/^\d{2}:\d{2}:\d{2}$/);
  });
});
