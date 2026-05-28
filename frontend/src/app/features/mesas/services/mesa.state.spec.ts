import { TestBed } from '@angular/core/testing';

import { MesaState } from './mesa.state';

describe('MesaState', () => {
  let service: MesaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MesaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
