import { TestBed } from '@angular/core/testing';

import { ComandaService } from './comanda-service';

describe('ComandaService', () => {
  let service: ComandaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComandaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
