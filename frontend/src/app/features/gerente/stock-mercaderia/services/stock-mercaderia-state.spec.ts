import { TestBed } from '@angular/core/testing';

import { StockMercaderiaState } from './stock-mercaderia-state';

describe('StockMercaderiaState', () => {
  let service: StockMercaderiaState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockMercaderiaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
