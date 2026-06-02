import { TestBed } from '@angular/core/testing';

import { CartaState } from './carta-state';

describe('CartaState', () => {
  let service: CartaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CartaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
