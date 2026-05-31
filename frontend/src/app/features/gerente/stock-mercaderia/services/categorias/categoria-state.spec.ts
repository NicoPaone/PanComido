import { TestBed } from '@angular/core/testing';

import { CategoriaState } from './categoria-state';

describe('CategoriaState', () => {
  let service: CategoriaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CategoriaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
