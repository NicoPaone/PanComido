import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetodosPagoList } from './metodos-pago-list';

describe('MetodosPagoList', () => {
  let component: MetodosPagoList;
  let fixture: ComponentFixture<MetodosPagoList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MetodosPagoList],
    }).compileComponents();

    fixture = TestBed.createComponent(MetodosPagoList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
