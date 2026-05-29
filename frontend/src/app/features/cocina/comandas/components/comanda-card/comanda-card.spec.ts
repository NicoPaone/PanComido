import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComandaCard } from './comanda-card';

describe('ComandaCard', () => {
  let component: ComandaCard;
  let fixture: ComponentFixture<ComandaCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaCard],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
