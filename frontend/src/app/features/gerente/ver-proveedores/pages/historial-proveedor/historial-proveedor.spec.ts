import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { HistorialProveedorComponent } from './historial-proveedor';
import { VerProveedoresState } from '../../services/ver-proveedores.state';
import { Proveedor, PedidoProveedor } from '../../../../../core/models/proveedor';
import { vi } from 'vitest';

describe('HistorialProveedorComponent', () => {
  let component: HistorialProveedorComponent;
  let stateMock: any;
  let routerMock: any;

  const mockProveedor: Proveedor = {
    id: 1,
    nombre: 'Distribuidora Sur',
    contacto: 'Mariela Gómez',
    telefono: '+54 11 5555-1200',
    email: 'ventas@distribuidorasur.com',
    direccion: 'Av. San Martín 1200, CABA',
    activo: true,
    fechaUltimoPedido: '2026-05-18T09:00:00.000Z',
    categorias: ['Carne', 'Verdura']
  };

  const mockPedido: PedidoProveedor = {
    id: 101,
    fecha: '2026-05-18T09:00:00.000Z',
    concepto: 'Pedido de carnes y verduras',
    monto: 184500,
    estado: 'Recibido',
    observacion: 'Recepción completa en cámaras',
    items: [
      { id: '6', nombre: 'Bife de Chorizo', cantidad: 10, unidadMedida: 'KG' }
    ]
  };

  beforeEach(() => {
    stateMock = {
      proveedorSeleccionado: vi.fn().mockReturnValue(mockProveedor),
      historialProveedor: vi.fn().mockReturnValue([mockPedido]),
      loadingHistorial: vi.fn().mockReturnValue(false),
      pedidoHistorialSeleccionado: vi.fn().mockReturnValue(null),
      proveedores: vi.fn().mockReturnValue([mockProveedor]),
      cargarDatos: vi.fn(),
      seleccionarProveedor: vi.fn(),
      cargarHistorial: vi.fn(),
      abrirDetallePedido: vi.fn(),
      cerrarDetallePedido: vi.fn()
    };

    routerMock = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [HistorialProveedorComponent],
      providers: [
        { provide: VerProveedoresState, useValue: stateMock },
        { provide: Router, useValue: routerMock },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => '1' } } }
        }
      ]
    });

    const fixture = TestBed.createComponent(HistorialProveedorComponent);
    component = fixture.componentInstance;
  });

  it('debería crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debería seleccionar el proveedor y cargar el historial con el id de la ruta', () => {
      component.ngOnInit();
      expect(stateMock.seleccionarProveedor).toHaveBeenCalledWith(1);
      expect(stateMock.cargarHistorial).toHaveBeenCalledWith(1);
    });

    it('debería cargar los datos si la lista de proveedores está vacía', () => {
      stateMock.proveedores = vi.fn().mockReturnValue([]);
      component.ngOnInit();
      expect(stateMock.cargarDatos).toHaveBeenCalled();
    });

    it('no debería llamar a cargarDatos si los proveedores ya están cargados', () => {
      component.ngOnInit();
      expect(stateMock.cargarDatos).not.toHaveBeenCalled();
    });

    it('debería navegar a ver-proveedores si no hay id en la ruta', () => {
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [HistorialProveedorComponent],
        providers: [
          { provide: VerProveedoresState, useValue: stateMock },
          { provide: Router, useValue: routerMock },
          {
            provide: ActivatedRoute,
            useValue: { snapshot: { paramMap: { get: () => null } } }
          }
        ]
      });
      const fixture2 = TestBed.createComponent(HistorialProveedorComponent);
      const comp2 = fixture2.componentInstance;
      comp2.ngOnInit();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
    });
  });

  describe('volver', () => {
    it('debería navegar a ver-proveedores', () => {
      component.volver();
      expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
    });
  });

  describe('getEstadoClase', () => {
    it('debería devolver "estado-recibido" para Recibido', () => {
      expect(component.getEstadoClase('Recibido')).toBe('estado-recibido');
    });
    it('debería devolver "estado-enviado" para Enviado', () => {
      expect(component.getEstadoClase('Enviado')).toBe('estado-enviado');
    });
    it('debería devolver "estado-pendiente" para Pendiente', () => {
      expect(component.getEstadoClase('Pendiente')).toBe('estado-pendiente');
    });
    it('debería devolver "estado-pendiente" para Pendiente', () => {
      expect(component.getEstadoClase('Pendiente')).toBe('estado-pendiente');
    });
  });

  describe('abrirDetallePedido y cerrarDetallePedido', () => {
    it('debería llamar a state.abrirDetallePedido con el pedido', () => {
      component.abrirDetallePedido(mockPedido);
      expect(stateMock.abrirDetallePedido).toHaveBeenCalledWith(mockPedido);
    });

    it('debería llamar a state.cerrarDetallePedido', () => {
      component.cerrarDetallePedido();
      expect(stateMock.cerrarDetallePedido).toHaveBeenCalled();
    });
  });
});
