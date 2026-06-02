import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KdsContadorTiempo } from './kds-contador-tiempo';

describe('KdsContadorTiempo', () => {
  let component: KdsContadorTiempo;
  let fixture: ComponentFixture<KdsContadorTiempo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KdsContadorTiempo],
    }).compileComponents();

    fixture = TestBed.createComponent(KdsContadorTiempo);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
