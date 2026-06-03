import { TestBed } from '@angular/core/testing';

import { CartaService } from './carta-service';

describe('CartaService', () => {
  let service: CartaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CartaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
