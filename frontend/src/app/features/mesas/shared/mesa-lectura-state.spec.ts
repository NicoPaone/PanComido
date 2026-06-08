import { TestBed } from '@angular/core/testing';
import { MesaLecturaState } from './mesa-lectura-state';
import { MesaService } from '../services/mesa.service';
import { of } from 'rxjs';

describe('MesaLecturaState', () => {
  let service: MesaLecturaState;
  let mesaServiceMock: any;

  beforeEach(() => {
    mesaServiceMock = {
      getMesas: vi.fn().mockReturnValue(of([])),
      ocuparMesa: vi.fn(),
      cambiarEstado: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        { provide: MesaService, useValue: mesaServiceMock }
      ]
    });
    service = TestBed.inject(MesaLecturaState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
