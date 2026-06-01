import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BotonComensal } from './boton-comensal';

describe('BotonComensal', () => {
  let component: BotonComensal;
  let fixture: ComponentFixture<BotonComensal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BotonComensal],
    }).compileComponents();

    fixture = TestBed.createComponent(BotonComensal);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
