import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TurnosLaboralesList } from './turnos-laborales-list';

describe('TurnosLaboralesList', () => {
  let component: TurnosLaboralesList;
  let fixture: ComponentFixture<TurnosLaboralesList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TurnosLaboralesList],
    }).compileComponents();

    fixture = TestBed.createComponent(TurnosLaboralesList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
