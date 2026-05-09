import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Prueba } from './prueba';

describe('Prueba', () => {
  let component: Prueba;
  let fixture: ComponentFixture<Prueba>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Prueba],
    }).compileComponents();

    fixture = TestBed.createComponent(Prueba);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
