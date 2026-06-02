import { TestBed } from '@angular/core/testing';

import { LlamadoService } from './llamado-service';

describe('LlamadoService', () => {
  let service: LlamadoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LlamadoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
