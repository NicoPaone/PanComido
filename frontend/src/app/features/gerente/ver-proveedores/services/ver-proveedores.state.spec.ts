import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { VerProveedoresStateService } from './ver-proveedores.state';
import { VerProveedoresApiService } from './ver-proveedores.api';
import { Proveedor, PedidoProveedor } from '../../../../core/models/proveedor';
import { Insumo as ProductoStockMock } from '../../../../core/models/producto-stock';
import { vi } from 'vitest';

describe('VerProveedoresStateService', () => {
  let service: VerProveedoresStateService;
  let apiServiceMock: any;

  const mockProveedores: Proveedor[] = [
    {
      id: 1,
      nombre: 'Distribuidora Sur',
      contacto: 'Mariela Gómez',
      telefono: '+54 11 5555-1200',
      email: 'ventas@distribuidorasur.com',
      direccion: 'Av. San Martín 1200, CABA',
      activo: true,
      fechaUltimoPedido: '2026-05-18T09:00:00.000Z',
      categorias: ['Carne', 'Verdura'],
      historialPedidos: []
    },
    {
      id: 2,
      nombre: 'Proveeduría El Molino',
      contacto: 'Lucas Ferreyra',
      telefono: '+54 11 4444-8800',
      email: 'pedidos@elmolino.com.ar',
      direccion: 'Ruta 8 km 23, Buenos Aires',
      activo: true,
      fechaUltimoPedido: '2026-05-12T15:45:00.000Z',
      categorias: ['Almacen'],
      historialPedidos: []
    }
  ];

  const mockProductos: ProductoStockMock[] = [
    { id: 1, nombre: 'Ajo', stock: 10, stockMinimo: 5, unidadMedida: 'KG', categoriaIngrediente: 'Verdura', fechaVencimiento: '2026-06-30' },
    { id: 2, nombre: 'Cebolla', stock: 2, stockMinimo: 8, unidadMedida: 'KG', categoriaIngrediente: 'Verdura', fechaVencimiento: '2026-06-30' }
  ];

  beforeEach(() => {
    apiServiceMock = {
      getProveedores: vi.fn().mockReturnValue(of([...mockProveedores])),
      getProductosDisponibles: vi.fn().mockReturnValue(of([...mockProductos])),
      crearPedidoProveedor: vi.fn().mockImplementation((id, pedido) => {
        const prov = mockProveedores.find(p => p.id === id);
        if (!prov) throw new Error('Not Found');
        const updatedProv: Proveedor = {
          ...prov,
          fechaUltimoPedido: new Date().toISOString(),
          historialPedidos: [
            {
              id: Date.now(),
              fecha: new Date().toISOString(),
              concepto: pedido.concepto,
              monto: pedido.monto,
              estado: 'Pendiente',
              observacion: pedido.observacion,
              items: pedido.items
            },
            ...prov.historialPedidos
          ]
        };
        return of(updatedProv);
      })
    };

    TestBed.configureTestingModule({
      providers: [
        VerProveedoresStateService,
        { provide: VerProveedoresApiService, useValue: apiServiceMock }
      ]
    });

    service = TestBed.inject(VerProveedoresStateService);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('debería cargar proveedores y productos al ejecutar cargarDatos()', () => {
    expect(service.loading()).toBe(false);
    service.cargarDatos();
    expect(apiServiceMock.getProveedores).toHaveBeenCalled();
    expect(apiServiceMock.getProductosDisponibles).toHaveBeenCalled();
    expect(service.proveedores()).toHaveLength(2);
    expect(service.productos()).toHaveLength(2);
    expect(service.proveedorSeleccionadoId()).toBe(1);
    expect(service.loading()).toBe(false);
  });

  it('debería filtrar proveedores por término de búsqueda', () => {
    service.cargarDatos();
    service.termino.set('Sur');
    expect(service.proveedoresFiltrados()).toHaveLength(1);
    expect(service.proveedoresFiltrados()[0].nombre).toBe('Distribuidora Sur');
  });

  it('debería ordenar los proveedores en proveedoresFiltrados con fecha de último pedido más reciente primero', () => {
    service.cargarDatos();
    const filtrados = service.proveedoresFiltrados();
    expect(filtrados[0].id).toBe(1); // 18/05 vs 12/05
    expect(filtrados[1].id).toBe(2);
  });

  it('debería seleccionar un proveedor correctamente', () => {
    service.cargarDatos();
    service.seleccionarProveedor(2);
    expect(service.proveedorSeleccionadoId()).toBe(2);
    expect(service.proveedorSeleccionado()?.nombre).toBe('Proveeduría El Molino');
  });

  it('debería abrir el panel de pedido y de historial', () => {
    service.abrirPedido(2);
    expect(service.proveedorSeleccionadoId()).toBe(2);
    expect(service.panelModo()).toBe('pedido');

    service.abrirHistorial(1);
    expect(service.proveedorSeleccionadoId()).toBe(1);
    expect(service.panelModo()).toBe('historial');
  });

  it('debería manejar la selección de detalle del pedido e historial', () => {
    const mockPedido: PedidoProveedor = {
      id: 101,
      fecha: '2026-05-18T09:00:00.000Z',
      concepto: 'Pedido de carnes',
      monto: 5000,
      estado: 'Recibido',
      observacion: 'Completo',
      items: []
    };
    service.abrirDetallePedido(mockPedido);
    expect(service.pedidoHistorialSeleccionado()).toEqual(mockPedido);

    service.cerrarDetallePedido();
    expect(service.pedidoHistorialSeleccionado()).toBeNull();
  });

  it('debería actualizar los signals al seleccionar un producto', () => {
    service.cargarDatos();
    const prod = mockProductos[0]; // Ajo (unidadMedida: KG)
    service.seleccionarProducto(prod);

    expect(service.productoSeleccionadoId()).toBe(1);
    expect(service.productoTexto()).toBe('Ajo');
    expect(service.cantidadProducto()).toBe(0.1); // min de KG es 0.1
    expect(service.precioProductoManual()).toBe(1200); // de preciosMock['1']
  });

  it('debería autocompletar el producto al cambiar el texto de búsqueda por coincidencia exacta', () => {
    service.cargarDatos();
    service.onProductoTextoChange('Cebolla');
    expect(service.productoSeleccionadoId()).toBe(2);
    expect(service.precioProductoManual()).toBe(900); // de preciosMock['2']
  });

  it('debería resetear el producto seleccionado si el texto de búsqueda no tiene coincidencia exacta', () => {
    service.cargarDatos();
    service.onProductoTextoChange('Cebo');
    expect(service.productoSeleccionadoId()).toBeNull();
    expect(service.precioProductoManual()).toBeNull();
  });

  it('debería agregar un ingrediente del catálogo al pedido y calcular el montoEstimado', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]); // Ajo, precio 1200, cantidad 0.1
    service.cantidadProducto.set(2); // cambiar cantidad a 2 KG
    service.agregarItemPedido();

    expect(service.pedidoItems()).toHaveLength(1);
    expect(service.pedidoItems()[0]).toEqual({
      id: 1,
      nombre: 'Ajo',
      cantidad: 2,
      unidadMedida: 'KG',
      precioUnitario: 1200
    });
    expect(service.montoEstimado()).toBe(2400); // 2 * 1200
  });

  it('debería acumular cantidad si se agrega el mismo producto dos veces', () => {
    service.cargarDatos();
    // Primer agregado
    service.seleccionarProducto(mockProductos[0]);
    service.cantidadProducto.set(1.5);
    service.agregarItemPedido();

    // Segundo agregado
    service.seleccionarProducto(mockProductos[0]);
    service.cantidadProducto.set(2.5);
    service.agregarItemPedido();

    expect(service.pedidoItems()).toHaveLength(1);
    expect(service.pedidoItems()[0].cantidad).toBe(4);
    expect(service.montoEstimado()).toBe(4800); // 4 * 1200
  });

  it('debería permitir agregar ingredientes manuales (no presentes en catálogo)', () => {
    service.cargarDatos();
    service.productoTexto.set('Perejil');
    service.precioProductoManual.set(300);
    service.cantidadProducto.set(3);
    service.agregarItemPedido();

    expect(service.pedidoItems()).toHaveLength(1);
    expect(service.pedidoItems()[0].id).toBe('manual-perejil');
    expect(service.pedidoItems()[0].nombre).toBe('Perejil');
    expect(service.pedidoItems()[0].cantidad).toBe(3);
    expect(service.pedidoItems()[0].precioUnitario).toBe(300);
    expect(service.montoEstimado()).toBe(900);
  });

  it('debería permitir actualizar la cantidad de un ingrediente y eliminarlo', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.agregarItemPedido(); // cantidad 0.1, precio 1200

    service.actualizarCantidadItem(1, 5);
    expect(service.pedidoItems()[0].cantidad).toBe(5);
    expect(service.montoEstimado()).toBe(6000);

    service.eliminarItemPedido(1);
    expect(service.pedidoItems()).toHaveLength(0);
    expect(service.montoEstimado()).toBe(0);
  });

  it('debería limpiar el pedido completo al llamar a limpiarPedido()', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.agregarItemPedido();
    service.observacionPedido.set('Urgente');

    service.limpiarPedido();
    expect(service.pedidoItems()).toHaveLength(0);
    expect(service.observacionPedido()).toBe('');
    expect(service.productoTexto()).toBe('');
  });

  it('debería enviar el pedido correctamente, actualizar la lista de proveedores, resetear el pedido y abrir whatsapp', () => {
    const openSpy = vi.spyOn(window, 'open').mockImplementation(() => null);

    service.cargarDatos(); // Selecciona proveedor 1
    service.seleccionarProducto(mockProductos[0]);
    service.cantidadProducto.set(10);
    service.agregarItemPedido();
    service.observacionPedido.set('Entregar por la tarde');

    service.enviarPedido();

    expect(apiServiceMock.crearPedidoProveedor).toHaveBeenCalledWith(1, {
      proveedorId: 1,
      concepto: 'Pedido de insumos',
      monto: 12000,
      observacion: 'Entregar por la tarde',
      items: [
        { id: 1, nombre: 'Ajo', cantidad: 10, unidadMedida: 'KG', precioUnitario: 1200 }
      ]
    });

    expect(openSpy).toHaveBeenCalled();
    const urlOpened = openSpy.mock.calls[0][0] as string;
    expect(urlOpened).toContain('https://wa.me/541155551200'); // Teléfono limpio de Mariela Gómez
    expect(urlOpened).toContain(encodeURIComponent('Ajo'));

    expect(service.pedidoItems()).toHaveLength(0);
    expect(service.panelModo()).toBe('historial');
    expect(service.mensajeAccion()).toBe('Pedido agregado correctamente');
    expect(service.proveedores().find(p => p.id === 1)?.historialPedidos).toHaveLength(1);

    openSpy.mockRestore();
  });
});
