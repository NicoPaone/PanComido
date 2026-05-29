import { TestBed } from '@angular/core/testing';

import { MesaStateService } from './mesa.state';

describe('MesaState', () => {
  let service: MesaStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MesaStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
