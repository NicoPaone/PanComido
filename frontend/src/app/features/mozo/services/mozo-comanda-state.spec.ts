import { TestBed } from '@angular/core/testing';

import { MozoComandaState } from './mozo-comanda-state';

describe('MozoComandaState', () => {
  let service: MozoComandaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MozoComandaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
