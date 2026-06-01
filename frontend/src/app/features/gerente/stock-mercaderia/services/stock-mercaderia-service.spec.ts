import { TestBed } from '@angular/core/testing';

import { StockMercaderiaService } from './insumos/stock-mercaderia-service';

describe('StockMercaderiaService', () => {
  let service: StockMercaderiaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockMercaderiaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
