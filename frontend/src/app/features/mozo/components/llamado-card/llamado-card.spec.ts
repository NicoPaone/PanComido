import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LlamadoCard } from './llamado-card';

describe('LlamadoCard', () => {
  let component: LlamadoCard;
  let fixture: ComponentFixture<LlamadoCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LlamadoCard],
    }).compileComponents();

    fixture = TestBed.createComponent(LlamadoCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
