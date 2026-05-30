import { TestBed } from '@angular/core/testing';

import { ComandoVozService } from './comando-voz.service';

describe('ComandoVoz', () => {
  let service: ComandoVozService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComandoVozService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
