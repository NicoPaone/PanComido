import { Injectable, signal } from '@angular/core';
import { delay, Observable, of } from 'rxjs';
import { environment } from '../../../../../environments/environment';

// Modelos de Dominio Estrictos
import { NuevoPedidoProveedor, NuevoProveedor, PedidoProveedor, Proveedor, SugerenciaPedidoItem } from '../../../../core/models/proveedor';
import { Insumo, INSUMOS_MOCK } from '../../../../core/models/insumos/insumo';

export const PROVEEDOR_ENDPOINTS = {
  base: `${environment.apiUrl}/proveedores`,
  byId: (id: number) => `${environment.apiUrl}/proveedores/${id}`,
  historial: (id: number) => `${environment.apiUrl}/proveedores/${id}/historial`,
  pedidos: (id: number) => `${environment.apiUrl}/proveedores/${id}/pedidos`,
  crearPedido: (id: number) => `${environment.apiUrl}/proveedores/${id}/pedidos`,
  crearProveedor: `${environment.apiUrl}/proveedores`,
  productos: `${environment.apiUrl}/proveedores/productos-disponibles`,
  sugerenciaIA: (id: number) => `${environment.apiUrl}/proveedores/${id}/pedido-sugerido-ia`
};

// 🔥 FIX: Actualizado al modelo estricto de PedidoProveedor
const PROVEEDORES_MOCK: Proveedor[] = [
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
    historialPedidos: [
      {
        id: 101,
        fecha: '2026-05-18T09:00:00.000Z',
        concepto: 'Pedido de carnes y verduras',
        monto: 184500,
        estado: 'Recibido',
        observacion: 'Recepción completa en cámaras',
        items: [
          // ✅ FIX: El ID ahora es number, y unidadMedida es un objeto
          { id: 6, nombre: 'Bife de Chorizo', cantidad: 10, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 18450 },
          { id: 5, nombre: 'Tomate Perita', cantidad: 12, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 0 }
        ]
      },
      {
        id: 102,
        fecha: '2026-04-28T10:30:00.000Z',
        concepto: 'Reposición semanal',
        monto: 132000,
        estado: 'Confirmado',
        observacion: 'En preparación',
        items: [
          { id: 4, nombre: 'Harina 0000', cantidad: 20, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 6600 }
        ]
      }
    ]
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
    historialPedidos: [
      {
        id: 201,
        fecha: '2026-05-12T15:45:00.000Z',
        concepto: 'Harinas y secos',
        monto: 97500,
        estado: 'Recibido',
        observacion: 'Entrega sin incidencias',
        items: [
          { id: 4, nombre: 'Harina 0000', cantidad: 25, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 3900 }
        ]
      }
    ]
  }
];

@Injectable({
  providedIn: 'root'
})
export class ProveedorService {
  private readonly proveedoresSignal = signal<Proveedor[]>(PROVEEDORES_MOCK);
  private readonly productosSignal = signal<Insumo[]>(INSUMOS_MOCK);

  getProveedores(): Observable<Proveedor[]> {
    return of(this.proveedoresSignal()).pipe(delay(180));
  }

  getProveedorById(id: number): Observable<Proveedor | undefined> {
    return of(this.proveedoresSignal().find(proveedor => proveedor.id === id)).pipe(delay(120));
  }

  getHistorialProveedor(id: number): Observable<PedidoProveedor[]> {
    const proveedor = this.proveedoresSignal().find(item => item.id === id);
    return of(proveedor?.historialPedidos ?? []).pipe(delay(120));
  }

  getProductosDisponibles(): Observable<Insumo[]> {
    return of(this.productosSignal()).pipe(delay(120));
  }

  crearPedidoProveedor(id: number | string, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    const fechaPedido = new Date().toISOString();
    
    const nuevoPedido: PedidoProveedor = {
      id: Date.now(),
      fecha: fechaPedido,
      concepto: pedido.concepto,
      monto: pedido.monto,
      estado: 'Pendiente',
      observacion: pedido.observacion,
     items: pedido.items.map(item => ({
        id: Number(item.id),
        nombre: item.nombre ?? 'Insumo',
        cantidad: item.cantidad,

        unidadMedida: item.unidadMedida ?? { id: 0, nombre: 'UN' }, 
        
        precioUnitario: item.precioUnitario ?? 0
      }))
    };

    this.proveedoresSignal.update(proveedores =>
      proveedores.map(proveedor => {
        if (proveedor.id !== Number(id)) {
          return proveedor;
        }

        return {
          ...proveedor,
          fechaUltimoPedido: fechaPedido,
          historialPedidos: [nuevoPedido, ...(proveedor.historialPedidos ?? [])]
        };
      })
    );

    const actualizado = this.proveedoresSignal().find(proveedor => proveedor.id === Number(id))!;
    return of(actualizado).pipe(delay(180));
  }

  registrarPedido(id: number, pedido: NuevoPedidoProveedor): Observable<Proveedor> {
    return this.crearPedidoProveedor(id, pedido);
  }

  crearProveedor(proveedor: NuevoProveedor): Observable<Proveedor> {
    const direccion = [proveedor.calle, proveedor.numero].filter(Boolean).join(' ').trim();
    const direccionCompleta = direccion + (proveedor.ciudad ? `, ${proveedor.ciudad}` : '');

    const nuevoProveedor: Proveedor = {
      id: Date.now(),
      nombre: proveedor.nombre,
      contacto: proveedor.contacto,
      telefono: proveedor.telefono,
      email: proveedor.email,
      direccion: direccionCompleta,
      activo: true,
      fechaUltimoPedido: null,
      historialPedidos: [],
      categorias: proveedor.categorias ?? []
    };

    this.proveedoresSignal.update(proveedores => [nuevoProveedor, ...proveedores]);
    return of(nuevoProveedor).pipe(delay(180));
  }

  getPedidoSugeridoIA(id: number): Observable<SugerenciaPedidoItem[]> {
    const proveedor = this.proveedoresSignal().find(p => p.id === id);
    if (!proveedor) {
      return of([]);
    }

    const providerCats = proveedor.categorias ?? [];

    const costosMock: Record<string, number> = {
      '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100, 
      '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
    };

    const sugeridos: SugerenciaPedidoItem[] = INSUMOS_MOCK
      .filter(prod => providerCats.includes(prod.categoriaIngrediente.descripcion))
      .filter(prod => prod.stock < prod.stockMinimo * 1.5)
      .map(prod => {
        const cantidadSugerida = Math.max(1, Math.round(prod.stockMinimo * 2 - prod.stock));
        const consumoEstimado30Dias = prod.stockMinimo * 3;
        const precioUnitario = costosMock[prod.id.toString()] ?? 500;

        return {
          productoId: prod.id.toString(),
          nombre: prod.nombre,
          // ✅ FIX: Pasamos el objeto UnidadMedida completo
          unidadMedida: prod.unidadMedida, 
          stockActual: prod.stock,
          stockMinimo: prod.stockMinimo,
          consumoEstimado30Dias,
          cantidadSugerida,
          precioUnitario
        } as SugerenciaPedidoItem;
      });

    return of(sugeridos).pipe(delay(250));
  }
}