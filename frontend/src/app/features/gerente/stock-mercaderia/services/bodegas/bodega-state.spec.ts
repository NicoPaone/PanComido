import { TestBed } from '@angular/core/testing';

import { BodegaState } from './bodega-state';

describe('BodegaState', () => {
  let service: BodegaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BodegaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
