import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { VerProveedoresState } from './ver-proveedores.state';
import { VerProveedoresApiService } from './ver-proveedores.api';
import { Proveedor, PedidoProveedor } from '../../../../core/models/domain/proveedor';
import { Insumo } from '../../../../core/models/domain/insumo';

describe('VerProveedoresState', () => {
  let service: VerProveedoresState;
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

  const mockProductos: Insumo[] = [
    { id: 1, nombre: 'Ajo', stockActual: 10, stockMinimo: 5, unidadMedida: { id: 1, nombre: 'Kg' }, categoriaIngrediente: { id: 2, descripcion: 'Verdura', tipoAplica: 'Ingrediente' }, vencimiento: '2026-06-30' },
    { id: 2, nombre: 'Cebolla', stockActual: 2, stockMinimo: 8, unidadMedida: { id: 1, nombre: 'Kg' }, categoriaIngrediente: { id: 2, descripcion: 'Verdura', tipoAplica: 'Ingrediente' }, vencimiento: '2026-06-30' }
  ];

  beforeEach(() => {
    apiServiceMock = {
      getProveedores: vi.fn().mockReturnValue(of([...mockProveedores])),
      getProductosDisponibles: vi.fn().mockReturnValue(of([...mockProductos])),
      getBodegas: vi.fn().mockReturnValue(of([])),
      getHistorialPedidos: vi.fn().mockReturnValue(of([])),
      getInsumosProveedor: vi.fn().mockReturnValue(of([...mockProductos])),
      crearPedidoProveedor: vi.fn().mockImplementation((id, pedido) => {
        const prov = mockProveedores.find(p => p.id === id);
        if (!prov) throw new Error('Not Found');
        return of({
          id: Date.now(),
          fecha: new Date().toISOString(),
          concepto: pedido.concepto,
          monto: pedido.monto,
          estado: 'Pendiente',
          observacion: pedido.observacion,
          items: pedido.items
        });
      })
    };

    TestBed.configureTestingModule({
      providers: [
        VerProveedoresState,
        { provide: VerProveedoresApiService, useValue: apiServiceMock }
      ]
    });

    service = TestBed.inject(VerProveedoresState);
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('debería cargar proveedores al ejecutar cargarDatos()', () => {
    service.cargarDatos();
    expect(apiServiceMock.getProveedores).toHaveBeenCalled();
    expect(apiServiceMock.getBodegas).toHaveBeenCalled();
    expect(service.proveedores()).toHaveLength(2);
  });

  it('debería seleccionar un proveedor correctamente', () => {
    service.cargarDatos();
    service.seleccionarProveedor(2);
    expect(service.proveedorSeleccionadoId()).toBe(2);
  });

  it('debería abrir el panel de pedido y de historial', () => {
    service.abrirPedido(1);
    expect(service.proveedorSeleccionadoId()).toBe(1);
    expect(service.panelModo()).toBe('pedido');

    service.abrirHistorial(1);
    expect(service.panelModo()).toBe('historial');
  });

  it('debería actualizar los signals al seleccionar un producto', () => {
    service.cargarDatos();
    const prod = mockProductos[0];
    service.seleccionarProducto(prod);

    expect(service.productoSeleccionadoId()).toBe(1);
    expect(service.productoTexto()).toBe('Ajo');
  });

  it('debería autocompletar el producto al cambiar el texto de búsqueda por coincidencia exacta', () => {
    service.cargarDatos();
    service.onProductoTextoChange('Cebolla');
    expect(service.productoSeleccionadoId()).toBe(2);
  });

  it('debería agregar un ingrediente del catálogo al pedido y calcular el montoEstimado', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.cantidadProducto.set(2);
    service.precioProductoManual.set(1200);
    service.agregarItemPedido();

    expect(service.pedidoItems()).toHaveLength(1);
    expect(service.pedidoItems()[0].cantidad).toBe(2);
    expect(service.montoEstimado()).toBe(2400);
  });

  it('debería acumular cantidad si se agrega el mismo producto dos veces', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.precioProductoManual.set(1200);
    service.cantidadProducto.set(1.5);
    service.agregarItemPedido();

    service.seleccionarProducto(mockProductos[0]);
    service.precioProductoManual.set(1200);
    service.cantidadProducto.set(2.5);
    service.agregarItemPedido();

    expect(service.pedidoItems()).toHaveLength(1);
    expect(service.pedidoItems()[0].cantidad).toBe(4);
    expect(service.montoEstimado()).toBe(4800);
  });

  it('debería permitir actualizar la cantidad de un ingrediente y eliminarlo', () => {
    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.precioProductoManual.set(1200);
    service.agregarItemPedido();

    service.actualizarCantidadItem(1, 5);
    expect(service.pedidoItems()[0].cantidad).toBe(5);
    expect(service.montoEstimado()).toBe(6000);

    service.eliminarItemPedido(1);
    expect(service.pedidoItems()).toHaveLength(0);
    expect(service.montoEstimado()).toBe(0);
  });

  it('debería enviar el pedido correctamente', () => {
    const openSpy = vi.spyOn(window, 'open').mockImplementation(() => null);

    service.cargarDatos();
    service.seleccionarProducto(mockProductos[0]);
    service.cantidadProducto.set(10);
    service.precioProductoManual.set(1200);
    service.agregarItemPedido();
    service.observacionPedido.set('Entregar por la tarde');

    service.enviarPedido();

    expect(apiServiceMock.crearPedidoProveedor).toHaveBeenCalled();
    expect(service.pedidoItems()).toHaveLength(0);
    expect(service.mensajeAccion()).toContain('correctamente');

    openSpy.mockRestore();
  });
});
