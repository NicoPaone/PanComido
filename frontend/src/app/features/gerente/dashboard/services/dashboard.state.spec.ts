import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { DashboardStateService } from './dashboard.state';
import { DashboardApiService } from './dashboard.api';
import { of } from 'rxjs';
import { vi, expect, describe, it, beforeEach } from 'vitest';

describe('DashboardStateService', () => {
  let service: DashboardStateService;
  let apiSpy: any;

  beforeEach(() => {
    apiSpy = {
      getVencimientos: vi.fn().mockReturnValue(of([])),
      getRendimientoComercial: vi.fn().mockReturnValue(of({ masVendidos: [], menosVendidos: [] })),
      getResumenOperativo: vi.fn().mockReturnValue(of({
        totalVentas: '$ 0',
        totalPedidos: 0,
        ticketPromedio: '$ 0',
        promedioDiarioPedidos: 0,
        variacionVentas: '0%',
        variacionPedidos: '0%',
        variacionTicket: '0%',
        grafico: []
      }))
    };

    TestBed.configureTestingModule({
      providers: [
        DashboardStateService,
        { provide: DashboardApiService, useValue: apiSpy }
      ]
    });
    service = TestBed.inject(DashboardStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with default 7d period', () => {
    expect(service.periodo()).toBe('7d');
    expect(service.periodoLabel()).toBe('Ultima semana');
  });

  describe('setPeriodo', () => {
    it('should update periodo and call cargarDatos if not custom', () => {
      apiSpy.getResumenOperativo.mockClear();
      service.setPeriodo('30d');
      expect(service.periodo()).toBe('30d');
      expect(service.periodoLabel()).toBe('Ultimo mes');
      // cargarDatos triggers API calls
      expect(apiSpy.getResumenOperativo).toHaveBeenCalled();
    });

    it('should clear dates if changing to non-custom period', () => {
      service.setFechaDesde('01/01/2023');
      service.setFechaHasta('31/01/2023');
      expect(service.periodo()).toBe('custom');
      
      service.setPeriodo('7d');
      expect(service.fechaDesde()).toBe('');
      expect(service.fechaHasta()).toBe('');
    });
  });

  describe('setFechaDesde / setFechaHasta', () => {
    it('should set custom period and only load data when both dates are present', () => {
      apiSpy.getResumenOperativo.mockClear();
      
      service.setFechaDesde('01/01/2023');
      expect(service.periodo()).toBe('custom');
      expect(apiSpy.getResumenOperativo).not.toHaveBeenCalled(); // Missing hasta
      
      service.setFechaHasta('31/01/2023');
      expect(apiSpy.getResumenOperativo).toHaveBeenCalled();
    });
  });

  describe('Computed properties from ResumenOperativo', () => {
    beforeEach(() => {
      apiSpy.getResumenOperativo.mockReturnValue(of({
        totalVentas: '$ 1,500',
        totalPedidos: 15,
        ticketPromedio: '$ 100',
        promedioDiarioPedidos: 5,
        variacionVentas: '-10%',
        variacionPedidos: '+5%',
        variacionTicket: '-2%',
        grafico: [
          { etiqueta: '2023-01', total: '$ 500' },
          { etiqueta: '2023-02', total: '$ 1000' }
        ]
      }));
      service.cargarDatos(); // Refresh with new mock data
    });

    it('should correctly identify negative variations', () => {
      expect(service.variacionVentasEsNegativa()).toBe(true);
      expect(service.variacionPedidosEsNegativa()).toBe(false);
      expect(service.variacionTicketEsNegativa()).toBe(true);
    });

    it('should calculate ventasMensuales based on grafico data', () => {
      const mensuales = service.ventasMensuales();
      expect(mensuales.length).toBe(2);
      expect(mensuales[0].mes).toBe('Enero'); // 2023-01 parses to Enero
      expect(mensuales[0].ventas).toBe(500); // Extracted number
      expect(mensuales[1].mes).toBe('Febrero');
      expect(mensuales[1].ventas).toBe(1000);
    });

    it('should calculate maxVentasMensuales', () => {
      expect(service.maxVentasMensuales()).toBe(1000);
    });
  });

  describe('esModoCalendario', () => {
    it('should be true for 30d period', () => {
      service.setPeriodo('30d');
      expect(service.esModoCalendario()).toBe(true);
    });

    it('should be false for 7d period', () => {
      service.setPeriodo('7d');
      expect(service.esModoCalendario()).toBe(false);
    });

    it('should be true for custom period between 8 and 40 days', () => {
      // Stub the method or calculate actual dates. 
      service.setFechaDesde('01/01/2023');
      service.setFechaHasta('15/01/2023');
      expect(service.esModoCalendario()).toBe(true);
    });
  });

  describe('recomendacionOperativa', () => {
    it('should suggest maintaining track if no critical issues', () => {
      apiSpy.getVencimientos.mockReturnValue(of([
        { nombre: 'Tomate', fecha: '2023-12-01', cantidad: '5', criticidad: 'baja', relativo: '' }
      ]));
      service.cargarDatos();
      expect(service.recomendacionOperativa()).toContain('Sin insumos de prioridad alta');
    });

    it('should prioritize critical items', () => {
      apiSpy.getVencimientos.mockReturnValue(of([
        { nombre: 'Tomate', fecha: '2023-12-01', cantidad: '5', criticidad: 'alta', relativo: '' },
        { nombre: 'Cebolla', fecha: '2023-12-01', cantidad: '5', criticidad: 'alta', relativo: '' }
      ]));
      service.cargarDatos();
      expect(service.recomendacionOperativa()).toContain('Priorizar Tomate y Cebolla');
    });
  });
});
