import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MisComandasPage } from './mis-comandas-page';

describe('MisComandasPage', () => {
  let component: MisComandasPage;
  let fixture: ComponentFixture<MisComandasPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MisComandasPage],
    }).compileComponents();

    fixture = TestBed.createComponent(MisComandasPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
