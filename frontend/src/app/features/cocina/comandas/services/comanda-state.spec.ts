import { TestBed } from '@angular/core/testing';

import { ComandaState } from './comanda-state';

describe('ComandaState', () => {
  let service: ComandaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComandaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
