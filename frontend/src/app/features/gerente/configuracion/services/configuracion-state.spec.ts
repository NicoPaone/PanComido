import { TestBed } from '@angular/core/testing';

import { ConfiguracionState } from './configuracion-state';

describe('ConfiguracionState', () => {
  let service: ConfiguracionState;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConfiguracionState);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
