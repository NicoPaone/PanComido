import { TestBed } from '@angular/core/testing';

import { StockMercaderiaService } from './stock-mercaderia-service';
import { of } from 'rxjs';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { ApiService } from '../../../../../core/services/api-service';

describe('StockMercaderiaService', () => {
  let service: StockMercaderiaService;
  let mockApiService: any;

  const mockInsumo: Insumo = {
    id: 1,
    nombre: 'Harina',
    stockActual: 10,
    unidadMedida: 'KG',
    vencimiento: null,
    stockMinimo: 5,
    estadoStock: 'Normal',
    tipo: 'Almacen',
    categoria: 'Almacen'
  };

  beforeEach(() => {
    mockApiService = {
      get: vi.fn(),
      post: vi.fn(),
      put: vi.fn(),
      delete: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        StockMercaderiaService,
        { provide: ApiService, useValue: mockApiService }
      ]
    });

    service = TestBed.inject(StockMercaderiaService);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  describe('Métodos HTTP', () => {
    it('getStockMercaderia() debería llamar al endpoint correcto', () => {
      mockApiService.get.mockReturnValue(of([mockInsumo]));

      service.getStockMercaderia().subscribe();

      expect(mockApiService.get).toHaveBeenCalledWith('insumo');
    });

    it('getById() debería llamar con el id correcto', () => {
      mockApiService.get.mockReturnValue(of(mockInsumo));

      service.getById(5).subscribe();

      expect(mockApiService.get).toHaveBeenCalledWith('insumo/5');
    });

    it('crear() debería hacer POST', () => {
      const nuevoProducto = { nombre: 'Azúcar', stockActual: 20 } as Partial<Insumo>;
      mockApiService.post.mockReturnValue(of(mockInsumo));

      service.crear(nuevoProducto).subscribe();

      expect(mockApiService.post).toHaveBeenCalledWith('insumo', nuevoProducto);
    });

    it('actualizar() debería hacer PUT con id', () => {
      mockApiService.put.mockReturnValue(of(mockInsumo));

      service.actualizar(10, mockInsumo).subscribe();

      expect(mockApiService.put).toHaveBeenCalledWith('insumo/10', mockInsumo);
    });

    it('eliminar() debería hacer DELETE', () => {
      mockApiService.delete.mockReturnValue(of(void 0));

      service.eliminar(7).subscribe();

      expect(mockApiService.delete).toHaveBeenCalledWith('insumo/7');
    });
  });
});