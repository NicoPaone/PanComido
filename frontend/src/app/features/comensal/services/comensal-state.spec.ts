import { TestBed } from '@angular/core/testing';

import { ComensalState } from './comensal-state';

describe('ComensalState', () => {
  let service: ComensalState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComensalState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
