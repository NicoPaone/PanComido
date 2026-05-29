import { TestBed } from '@angular/core/testing';

import { StockMercaderiaClient } from './stock-mercaderia-service';

describe('StockMercaderiaClient', () => {
  let service: StockMercaderiaClient;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockMercaderiaClient);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
