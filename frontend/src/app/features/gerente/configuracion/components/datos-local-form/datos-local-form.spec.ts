import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DatosLocalForm } from './datos-local-form';

describe('DatosLocalForm', () => {
  let component: DatosLocalForm;
  let fixture: ComponentFixture<DatosLocalForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DatosLocalForm],
    }).compileComponents();

    fixture = TestBed.createComponent(DatosLocalForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
