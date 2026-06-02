import { TestBed } from '@angular/core/testing';
import { SignalRConexionService } from './base-hub-service';


describe('BaseHubService', () => {
  let service: SignalRConexionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SignalRConexionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
