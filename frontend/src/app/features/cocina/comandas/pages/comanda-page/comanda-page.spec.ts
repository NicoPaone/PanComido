import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComandaPage } from './comanda-page';

describe('ComandaPage', () => {
  let component: ComandaPage;
  let fixture: ComponentFixture<ComandaPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaPage],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
