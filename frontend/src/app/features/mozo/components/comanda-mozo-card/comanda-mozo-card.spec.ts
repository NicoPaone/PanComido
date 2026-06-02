import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComandaMozoCard } from './comanda-mozo-card';

describe('ComandaMozoCard', () => {
  let component: ComandaMozoCard;
  let fixture: ComponentFixture<ComandaMozoCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComandaMozoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(ComandaMozoCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
