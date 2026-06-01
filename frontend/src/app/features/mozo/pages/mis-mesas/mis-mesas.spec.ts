import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MisMesasPage } from './mis-mesas';

describe('MisMesas', () => {
  let component: MisMesasPage;
  let fixture: ComponentFixture<MisMesasPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MisMesasPage],
    }).compileComponents();

    fixture = TestBed.createComponent(MisMesasPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
