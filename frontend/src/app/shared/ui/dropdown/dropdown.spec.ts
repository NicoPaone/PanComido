import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Stock } from './stock';

describe('ModificarCarta', () => {
  let component: ModificarCarta;
  let fixture: ComponentFixture<ModificarCarta>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModificarCarta],
    }).compileComponents();

    fixture = TestBed.createComponent(ModificarCarta);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
