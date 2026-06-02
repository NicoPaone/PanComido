import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LlamadosMozoPage } from './llamados-mozo-page';

describe('LlamadosMozoPage', () => {
  let component: LlamadosMozoPage;
  let fixture: ComponentFixture<LlamadosMozoPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LlamadosMozoPage],
    }).compileComponents();

    fixture = TestBed.createComponent(LlamadosMozoPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
