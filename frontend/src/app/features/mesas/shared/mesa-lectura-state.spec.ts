import { TestBed } from '@angular/core/testing';

import { MesaLecturaState } from './mesa-lectura-state';

describe('MesaLecturaState', () => {
  let service: MesaLecturaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MesaLecturaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
