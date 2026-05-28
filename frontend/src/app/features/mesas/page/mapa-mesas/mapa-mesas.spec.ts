import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapaMesas } from './mapa-mesas';

describe('MapaMesas', () => {
  let component: MapaMesas;
  let fixture: ComponentFixture<MapaMesas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MapaMesas],
    }).compileComponents();

    fixture = TestBed.createComponent(MapaMesas);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
