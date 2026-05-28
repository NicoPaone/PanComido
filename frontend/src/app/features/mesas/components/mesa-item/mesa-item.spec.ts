import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MesaItem } from './mesa-item';

describe('MesaItem', () => {
  let component: MesaItem;
  let fixture: ComponentFixture<MesaItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MesaItem],
    }).compileComponents();

    fixture = TestBed.createComponent(MesaItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
