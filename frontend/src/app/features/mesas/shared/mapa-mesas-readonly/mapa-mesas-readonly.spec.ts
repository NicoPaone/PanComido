import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapaMesasReadonly } from './mapa-mesas-readonly';

describe('MapaMesasReadonly', () => {
  let component: MapaMesasReadonly;
  let fixture: ComponentFixture<MapaMesasReadonly>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MapaMesasReadonly],
    }).compileComponents();

    fixture = TestBed.createComponent(MapaMesasReadonly);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
