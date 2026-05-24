import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapaDeMesas } from './mapa-de-mesas';

describe('MapaDeMesas', () => {
  let component: MapaDeMesas;
  let fixture: ComponentFixture<MapaDeMesas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MapaDeMesas],
    }).compileComponents();

    fixture = TestBed.createComponent(MapaDeMesas);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
