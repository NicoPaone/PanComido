import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';
import { RealizarPedidoSugeridoComponent } from './realizar-pedido-sugerido';
import { ProveedorApiService } from '../../services/proveedor.api';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/domain/proveedor';

describe('RealizarPedidoSugeridoComponent', () => {
  let component: RealizarPedidoSugeridoComponent;
  let fixture: ComponentFixture<RealizarPedidoSugeridoComponent>;
  let routerMock: any;
  let proveedorServiceMock: any;

  const mockProveedor: Proveedor = {
    id: 1,
    nombre: 'Distribuidora Sur',
    contacto: 'Mariela Gómez',
    telefono: '+54 11 5555-1200',
    email: 'ventas@distribuidorasur.com',
    direccion: 'Av. San Martín 1200, CABA',
    activo: true,
    fechaUltimoPedido: '2026-05-18T09:00:00.000Z',
    categorias: ['Verdura', 'Carne'],
    historialPedidos: []
  };

  const mockSugerencias: SugerenciaPedidoItem[] = [
    {
      productoId: '5',
      nombre: 'Tomate Perita',
      unidadMedida: { id: 1, nombre: 'Kg' },
      stockActual: 12,
      stockMinimo: 5,
      cantidadSugerida: 10,
      precioUnitario: 1100
    }
  ];

  beforeEach(() => {
    routerMock = {
      navigate: vi.fn()
    };

    proveedorServiceMock = {
      getProveedores: vi.fn().mockReturnValue(of([mockProveedor])),
      getProveedorById: vi.fn().mockReturnValue(of(mockProveedor)),
      getInsumosAReponer: vi.fn().mockReturnValue(of(mockSugerencias)),
      getProductosDisponibles: vi.fn().mockReturnValue(of([])),
      crearPedidoProveedor: vi.fn().mockReturnValue(of(mockProveedor)),
      getBodegas: vi.fn().mockReturnValue(of([])),
      getHistorialCantidadPedidos: vi.fn().mockReturnValue(of([])),
      getInsumosProveedor: vi.fn().mockReturnValue(of([])),
    };
  });

  async function createComponent(routeId: string | null = '1') {
    await TestBed.configureTestingModule({
      imports: [RealizarPedidoSugeridoComponent],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: vi.fn().mockReturnValue(routeId) } } } },
        { provide: ProveedorApiService, useValue: proveedorServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RealizarPedidoSugeridoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  }

  it('debería crearse correctamente', async () => {
    await createComponent();
    expect(component).toBeTruthy();
  });

  it('debería cargar datos con el id del proveedor', async () => {
    await createComponent();
    expect(proveedorServiceMock.getProveedores).toHaveBeenCalled();
  });

  it('debería cambiar el término de búsqueda al cambiar el buscador', async () => {
    await createComponent();
    component.onSearchChanged('Tomate');
    expect(component.busquedaProveedor()).toBe('Tomate');
  });

  it('debería navegar hacia atrás al llamar a volver', async () => {
    await createComponent();
    component.volver();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
  });

  it('debería enviar el pedido correctamente y navegar', async () => {
    await createComponent();
    component.observaciones.set('Observación personalizada');
    component.enviarPedido(mockProveedor);
    expect(proveedorServiceMock.crearPedidoProveedor).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalled();
  });
});
