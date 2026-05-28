import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormPlatoEditar } from './form-plato-editar';

describe('FormPlatoEditar', () => {
  let component: FormPlatoEditar;
  let fixture: ComponentFixture<FormPlatoEditar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormPlatoEditar],
    }).compileComponents();

    fixture = TestBed.createComponent(FormPlatoEditar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
