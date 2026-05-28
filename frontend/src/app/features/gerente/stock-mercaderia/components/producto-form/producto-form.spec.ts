import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductoForm } from './producto-form';

describe('ProductoForm', () => {
  let component: ProductoForm;
  let fixture: ComponentFixture<ProductoForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductoForm],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductoForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
