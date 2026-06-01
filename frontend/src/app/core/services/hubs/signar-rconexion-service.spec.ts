import { TestBed } from '@angular/core/testing';
import { SignalRConnectionService } from './signal-r-conexion-service';


describe('SignarRConexionService', () => {
  let service: SignalRConnectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SignalRConnectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
