import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModificarCartaComponent } from './modificar-carta';

describe('ModificarCartaComponent', () => {
  let component: ModificarCartaComponent;
  let fixture: ComponentFixture<ModificarCartaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModificarCartaComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ModificarCartaComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
