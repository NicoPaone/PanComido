import { TestBed } from '@angular/core/testing';
import { StockMercaderiaState } from './stock-mercaderia-state';
import { StockMercaderiaService } from './stock-mercaderia-service';
import { UnidadMedidaService } from '../unidad-medida.service';
import { CategoriaInsumoService } from '../categorias/categoria-insumo.service';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { Insumo } from '../../../../../core/models/domain/insumo';
import { INSUMOS_MOCK } from '../../../../../infra/mocks/insumo.mock';

describe('StockMercaderiaState', () => {
  let state: StockMercaderiaState;
  let mockService: any;
  let mockUnidadMedida: any;
  let mockCategoriaInsumo: any;

  beforeEach(() => {
    mockService = {
      getStockMercaderia: vi.fn(),
      crear: vi.fn(),
      actualizar: vi.fn(),
      eliminar: vi.fn()
    };

    mockUnidadMedida = {
      getUnidadesMedida: vi.fn().mockReturnValue(of([])),
    };

    mockCategoriaInsumo = {
      getCategorias: vi.fn().mockReturnValue(of([])),
    };

    TestBed.configureTestingModule({
      providers: [
        StockMercaderiaState,
        { provide: StockMercaderiaService, useValue: mockService },
        { provide: UnidadMedidaService, useValue: mockUnidadMedida },
        { provide: CategoriaInsumoService, useValue: mockCategoriaInsumo }
      ]
    });

    state = TestBed.inject(StockMercaderiaState);
  });

  it('debería crearse correctamente', () => {
    expect(state).toBeTruthy();
  });

  describe('Estado inicial', () => {
    it('debería tener arrays vacíos al inicio', () => {
      expect(state.productos()).toEqual([]);
      expect(state.cargando()).toBe(false);
    });
  });

  describe('cargarMercaderia()', () => {
    it('debería cargar productos y actualizar el signal', () => {
      mockService.getStockMercaderia.mockReturnValue(of(INSUMOS_MOCK));

      state.cargarMercaderia();

      expect(mockService.getStockMercaderia).toHaveBeenCalled();
      expect(state.productos()).toEqual(INSUMOS_MOCK);
      expect(state.cargando()).toBe(false);
    });

    it('debería manejar errores correctamente', () => {
      mockService.getStockMercaderia.mockReturnValue(throwError(() => new Error('Error de red')));

      state.cargarMercaderia();

      expect(state.cargando()).toBe(false);
    });
  });

  describe('guardarProducto()', () => {
    it('debería crear un nuevo producto', () => {
      const nuevo: Partial<Insumo> = {
        nombre: 'Sal',
        stockActual: 15,
        unidadMedida: { id: 1, nombre: 'Kg' },
        stockMinimo: 5,
        categoriaIngrediente: { id: 1, descripcion: 'Almacen', tipoAplica: 'Ingrediente' },
        vencimiento: '2028-01-01'
      };

      const productoCreado: Insumo = {
        id: 99,
        ...nuevo
      } as Insumo;

      mockService.crear.mockReturnValue(of(productoCreado));

      state.guardarProducto(nuevo as Insumo);

      expect(mockService.crear).toHaveBeenCalledWith(nuevo);
    });
  });

  describe('eliminarProducto()', () => {
    it('debería eliminar un producto', () => {
      mockService.eliminar.mockReturnValue(of(void 0));

      // Precargamos datos
      mockService.getStockMercaderia.mockReturnValue(of(INSUMOS_MOCK));
      state.cargarMercaderia();

      state.eliminarProducto(1);

      expect(mockService.eliminar).toHaveBeenCalledWith(1);
    });
  });

  describe('Computed Signals', () => {
    it('productosCriticos debería filtrar productos con stock bajo', () => {
      mockService.getStockMercaderia.mockReturnValue(of(INSUMOS_MOCK));
      state.cargarMercaderia();

      expect(state.productosCriticos().length).toBeGreaterThan(0);
      expect(state.cantidadProductosCriticos()).toBeGreaterThan(0);
    });
  });
});
