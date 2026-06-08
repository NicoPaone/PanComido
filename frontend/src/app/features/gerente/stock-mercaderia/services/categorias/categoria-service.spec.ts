import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { CategoriaInsumoService } from './categoria-insumo.service';
import { ApiService } from '../../../../../core/services/api-service';

describe('CategoriaService', () => {
  let service: CategoriaInsumoService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CategoriaInsumoService,
        { provide: ApiService, useValue: { get: vi.fn().mockReturnValue(of([])) } }
      ]
    });
    service = TestBed.inject(CategoriaInsumoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
