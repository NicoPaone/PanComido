import { TestBed } from '@angular/core/testing';

import { LlamadoState } from './llamado-state';

describe('LlamadoState', () => {
  let service: LlamadoState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LlamadoState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
