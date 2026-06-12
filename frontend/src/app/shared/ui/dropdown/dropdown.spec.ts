import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Dropdown } from './dropdown';

describe('Dropdown', () => {
  let component: Dropdown;
  let fixture: ComponentFixture<Dropdown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dropdown],
    }).compileComponents();

    fixture = TestBed.createComponent(Dropdown);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start closed', () => {
    expect(component.estadoAbierto()).toBe(false);
  });

  it('should toggle open/closed', () => {
    component.toggle();
    expect(component.estadoAbierto()).toBe(true);
    component.toggle();
    expect(component.estadoAbierto()).toBe(false);
  });

  it('should close when cerrar is called', () => {
    component.toggle();
    expect(component.estadoAbierto()).toBe(true);
    component.cerrar();
    expect(component.estadoAbierto()).toBe(false);
  });
});
