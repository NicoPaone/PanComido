import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InsumoPage } from './insumo-page';

describe('InsumoPage', () => {
  let component: InsumoPage;
  let fixture: ComponentFixture<InsumoPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InsumoPage],
    }).compileComponents();

    fixture = TestBed.createComponent(InsumoPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
