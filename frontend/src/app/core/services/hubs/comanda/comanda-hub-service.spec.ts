import { TestBed } from '@angular/core/testing';

import { ComandaHubService } from './comanda-hub-service';

describe('ComandaHubService', () => {
  let service: ComandaHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComandaHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
