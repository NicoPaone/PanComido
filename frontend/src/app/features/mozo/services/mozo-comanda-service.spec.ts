import { TestBed } from '@angular/core/testing';

import { MozoComandaService } from './mozo-comanda-service';

describe('MozoComandaService', () => {
  let service: MozoComandaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MozoComandaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
