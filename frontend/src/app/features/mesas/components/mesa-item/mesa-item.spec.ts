import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InsumoPage } from './insumo-page';
import { StockMercaderiaState } from '../../services/insumos/stock-mercaderia-state';
import { BodegaState } from '../../services/bodegas/bodega-state';
import { signal, computed } from '@angular/core';
import { Modal } from "../../../../../shared/ui/modal/modal";
import { Insumo } from '../../../../../core/models/insumos/insumo';

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { INSUMOS_MOCK } from '../../../../../core/models/insumos/insumo';

describe('InsumoPage', () => {
  let component: InsumoPage;
  let fixture: ComponentFixture<InsumoPage>;

  let mockStockState: any;
  let mockBodegaState: any;

  beforeEach(async () => {
    mockStockState = {
      productos: signal([...INSUMOS_MOCK]),
      categoriasUnicas: computed(() => ['Verdura', 'Almacen', 'Carne']),
      cargarMercaderia: vi.fn(),
      guardarProducto: vi.fn()
    };

    mockBodegaState = {
      bodegas: signal([]),
      cargarBodegasConInsumos: vi.fn(),
      cargarBodegas: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [InsumoPage],
      providers: [
        { provide: StockMercaderiaState, useValue: mockStockState },
        { provide: BodegaState, useValue: mockBodegaState }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MesaItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería inicializar cargando mercadería y bodegas', () => {
    expect(mockStockState.cargarMercaderia).toHaveBeenCalled();
    expect(mockBodegaState.cargarBodegasConInsumos).toHaveBeenCalled();
  });

  describe('Lógica de Filtrado', () => {
    it('debería filtrar por nombre', () => {
      component.termino.set('ajo');
      fixture.detectChanges();

      const filtrados = component.productosFiltrados();
      expect(filtrados.length).toBe(1);
      expect(filtrados[0].nombre).toBe('Ajo');
    });

    it('debería filtrar por categoría', () => {
      component.categoria.set('Verdura');
      fixture.detectChanges();

      const filtrados = component.productosFiltrados();
      expect(filtrados.length).toBeGreaterThan(0);
      expect(filtrados.every(p => p.categoria === 'Verdura')).toBe(true);
    });

    it('debería mostrar todos cuando no hay filtros', () => {
      component.termino.set('');
      component.categoria.set('Categorías');
      fixture.detectChanges();

      expect(component.productosFiltrados().length).toBe(INSUMOS_MOCK.length);
    });
  });

  describe('Gestión de Modales', () => {
    it('debería preparar el estado para crear un nuevo producto', () => {
      component.productoEditandoId.set(5);
      const mockModal = { abrir: vi.fn(), cerrar: vi.fn() } as any;

      component.abrirModalCrear(mockModal);

      expect(component.productoEditandoId()).toBeNull();
      expect(mockModal.abrir).toHaveBeenCalled();
      expect(component.tituloModal()).toBe('Nuevo Insumo');
    });

    it('debería preparar el estado para editar un producto existente', () => {
      const mockModal = { abrir: vi.fn(), cerrar: vi.fn() } as any;

      component.abrirModalEditar(mockModal, 2);

      expect(component.productoEditandoId()).toBe(2);
      expect(mockModal.abrir).toHaveBeenCalled();
      expect(component.tituloModal()).toBe('Editar Insumo');
      expect(component.productoSeleccionado()?.nombre).toBe('Cebolla');
    });

    it('debería guardar cambios y cerrar modal', () => {
      const mockModal = { cerrar: vi.fn() } as any;
      const datos = { id: 99, nombre: 'Azúcar', categoria: 'Almacen' } as Insumo;

      component.guardarCambios(datos, mockModal);

      expect(mockStockState.guardarProducto).toHaveBeenCalledWith(datos);
      expect(mockModal.cerrar).toHaveBeenCalled();
      expect(component.productoEditandoId()).toBeNull();
    });
  });
});
