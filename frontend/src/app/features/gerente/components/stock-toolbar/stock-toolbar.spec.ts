import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StockToolbar } from './stock-toolbar';

describe('StockToolbar', () => {
  let component: StockToolbar;
  let fixture: ComponentFixture<StockToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StockToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(StockToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
