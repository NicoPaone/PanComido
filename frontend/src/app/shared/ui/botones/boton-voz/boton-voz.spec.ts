import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BotonVoz } from './boton-voz';

describe('BotonVoz', () => {
  let component: BotonVoz;
  let fixture: ComponentFixture<BotonVoz>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BotonVoz],
    }).compileComponents();

    fixture = TestBed.createComponent(BotonVoz);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
