import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { vi } from 'vitest';
import { RealizarPedidoSugeridoComponent } from './realizar-pedido-sugerido';
import { RealizarPedidoSugeridoApiService } from '../services/realizar-pedido-sugerido.api';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';

describe('RealizarPedidoSugeridoComponent', () => {
  let component: RealizarPedidoSugeridoComponent;
  let fixture: ComponentFixture<RealizarPedidoSugeridoComponent>;
  let routerMock: any;
  let routeIdParam: string | null;
  let activatedRouteMock: any;
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
      unidadMedida: 'KG',
      stockActual: 12,
      stockMinimo: 5,
            cantidadSugerida: 10,
      precioUnitario: 1100
    }
  ];

  const mockProductos: ProductoStockMock[] = [
    {
      id: 1,
      nombre: 'Ajo',
      stock: 5,
      unidadMedida: 'KG',
      fechaVencimiento: '2026-05-17',
      stockMinimo: 2,
      categoriaIngrediente: 'Verdura'
    },
    {
      id: 4,
      nombre: 'Harina 0000',
      stock: 50,
      unidadMedida: 'KG',
      fechaVencimiento: '2026-12-05',
      stockMinimo: 15,
      categoriaIngrediente: 'Almacen'
    },
    {
      id: 5,
      nombre: 'Tomate Perita',
      stock: 12,
      unidadMedida: 'KG',
      fechaVencimiento: '2026-05-25',
      stockMinimo: 5,
      categoriaIngrediente: 'Verdura'
    },
    {
      id: 6,
      nombre: 'Bife de Chorizo',
      stock: 30,
      unidadMedida: 'KG',
      fechaVencimiento: '2026-05-28',
      stockMinimo: 10,
      categoriaIngrediente: 'Carne'
    }
  ];

  beforeEach(() => {
    routeIdParam = '1';
    
    routerMock = {
      navigate: vi.fn()
    };

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: vi.fn().mockImplementation((key: string) => routeIdParam)
        }
      }
    };

    proveedorServiceMock = {
      getProveedores: vi.fn().mockReturnValue(of([mockProveedor])),
      getProveedorById: vi.fn().mockReturnValue(of(mockProveedor)),
      getInsumosAReponer: vi.fn().mockReturnValue(of(mockSugerencias)),
      getProductosDisponibles: vi.fn().mockReturnValue(of(mockProductos)),
      crearPedidoProveedor: vi.fn().mockReturnValue(of(mockProveedor))
    };
  });

  async function createComponent() {
    await TestBed.configureTestingModule({
      imports: [RealizarPedidoSugeridoComponent],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: RealizarPedidoSugeridoApiService, useValue: proveedorServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RealizarPedidoSugeridoComponent);
    component = fixture.componentInstance;
  }

  it('debería crearse correctamente', async () => {
    await createComponent();
    fixture.detectChanges();
    expect(component).toBeTruthy();
    expect(component.proveedorId()).toBe(1);
    expect(component.proveedor()).toEqual(mockProveedor);
    expect(component.sugerencias()).toEqual(mockSugerencias);
    expect(component.pedidoItems()).toEqual(mockSugerencias);
    expect(component.productosDisponibles()).toEqual(mockProductos);
  });

  it('debería volver si el id del proveedor no es válido (NaN o <= 0)', async () => {
    routeIdParam = 'invalid';
    await createComponent();
    fixture.detectChanges();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
  });

  it('debería volver si el proveedor no existe', async () => {
    proveedorServiceMock.getProveedorById.mockReturnValue(of(null));
    await createComponent();
    fixture.detectChanges();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
  });

  it('debería calcular el monto estimado correctamente', async () => {
    await createComponent();
    fixture.detectChanges();
    // 10 * 1100 = 11000
    expect(component.montoEstimado()).toBe(11000);

    // Cambiamos cantidad o agregamos item
    component.pedidoItems.update(items => [
      ...items,
      {
        productoId: '6',
        nombre: 'Bife de Chorizo',
        unidadMedida: 'KG',
        stockActual: 30,
        stockMinimo: 10,
                cantidadSugerida: 2,
        precioUnitario: 7500
      }
    ]);
    // 11000 + (2 * 7500) = 26000
    expect(component.montoEstimado()).toBe(26000);
  });

  it('debería cambiar el término de búsqueda al cambiar el buscador', async () => {
    await createComponent();
    fixture.detectChanges();
    component.onSearchChanged('Tomate');
    expect(component.busqueda()).toBe('Tomate');
  });

  it('debería filtrar sugerencias extras por categorías y exclusión', async () => {
    await createComponent();
    fixture.detectChanges();
    
    // Buscar "Ajo" (Verdura, coincide con las categorías de proveedor: 'Verdura', 'Carne')
    // No está en pedidoItems (que solo tiene Tomate Perita, id: '5')
    component.onSearchChanged('Ajo');
    expect(component.sugerenciasExtras()).toEqual([
      expect.objectContaining({ id: 1, nombre: 'Ajo' })
    ]);

    // Buscar "Harina" (Almacen, NO coincide con las categorías de proveedor)
    component.onSearchChanged('Harina');
    expect(component.sugerenciasExtras()).toEqual([]);

    // Buscar "Tomate" (Verdura, coincide con categoría, pero YA está en pedidoItems)
    component.onSearchChanged('Tomate');
    expect(component.sugerenciasExtras()).toEqual([]);
  });

  it('debería agregar un producto manualmente', async () => {
    await createComponent();
    fixture.detectChanges();

    const prodAAgregar = mockProductos[0]; // Ajo
    component.onSearchChanged('Ajo');
    component.agregarProductoManual(prodAAgregar);

    expect(component.pedidoItems()).toHaveLength(2);
    expect(component.pedidoItems()[1]).toEqual(
      expect.objectContaining({
        productoId: '1',
        nombre: 'Ajo',
        cantidadSugerida: 1
      })
    );
    expect(component.busqueda()).toBe('');
  });

  it('debería eliminar un producto del pedido sugerido', async () => {
    await createComponent();
    fixture.detectChanges();

    component.eliminarItem('5');
    expect(component.pedidoItems()).toHaveLength(0);
  });

  it('debería actualizar la cantidad del producto y forzar mínimo 1 si el valor es inválido', async () => {
    await createComponent();
    fixture.detectChanges();

    const item = component.pedidoItems()[0];

    // Cambiar a cantidad válida
    component.onCantidadCambiada(item, 15);
    expect(component.pedidoItems()[0].cantidadSugerida).toBe(15);

    // Cambiar a cantidad inválida o nula
    component.onCantidadCambiada(item, null);
    expect(component.pedidoItems()[0].cantidadSugerida).toBe(1);

    component.onCantidadCambiada(item, -5);
    expect(component.pedidoItems()[0].cantidadSugerida).toBe(1);
  });

  it('debería navegar hacia atrás al llamar a volver', async () => {
    await createComponent();
    fixture.detectChanges();
    component.volver();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/staff', 'gerente', 'ver-proveedores']);
  });

  it('debería enviar el pedido correctamente y navegar', async () => {
    await createComponent();
    fixture.detectChanges();

    component.observaciones.set('Observación personalizada');
    component.enviarPedido();

    expect(proveedorServiceMock.crearPedidoProveedor).toHaveBeenCalledWith(
      1,
      expect.objectContaining({
        proveedorId: 1,
        concepto: 'Pedido sugerido',
        monto: 11000,
        observacion: 'Observación personalizada',
        items: [
          {
            id: '5',
            nombre: 'Tomate Perita',
            cantidad: 10,
            unidadMedida: 'KG',
            precioUnitario: 1100
          }
        ]
      })
    );

    expect(routerMock.navigate).toHaveBeenCalledWith(
      ['/staff', 'gerente', 'ver-proveedores'],
      { state: { created: true, message: 'Pedido sugerido enviado correctamente' } }
    );
  });

  it('debería no hacer nada al enviar pedido si no hay proveedor o está vacío', async () => {
    await createComponent();
    fixture.detectChanges();

    // Vaciar items
    component.pedidoItems.set([]);
    component.enviarPedido();
    expect(proveedorServiceMock.crearPedidoProveedor).not.toHaveBeenCalled();

    // Sin proveedor
    component.pedidoItems.set(mockSugerencias);
    component.proveedor.set(null);
    component.enviarPedido();
    expect(proveedorServiceMock.crearPedidoProveedor).not.toHaveBeenCalled();
  });
});
