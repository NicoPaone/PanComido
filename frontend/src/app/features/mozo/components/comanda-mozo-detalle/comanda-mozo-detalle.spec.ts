import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComandaMozoDetalle } from './comanda-mozo-detalle';

describe('ComandaMozoDetalle', () => {
  let component: ComandaMozoDetalle;
  let fixture: ComponentFixture<ComandaMozoDetalle>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaMozoDetalle],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaMozoDetalle);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
