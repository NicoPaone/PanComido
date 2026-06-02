import { TestBed } from '@angular/core/testing';

import { MozoHubService } from './mozo-hub-service';

describe('MozoHubService', () => {
  let service: MozoHubService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MozoHubService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
