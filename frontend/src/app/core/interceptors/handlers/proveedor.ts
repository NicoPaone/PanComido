import { HttpRequest, HttpResponse, HttpHandlerFn } from "@angular/common/http";
import { delay, Observable, of } from "rxjs";
import { Proveedor, NuevoPedidoProveedor, NuevoProveedor, PedidoProveedor } from "../../models/proveedor";
import { INSUMOS_MOCK, Insumo } from "../../models/insumos/insumo";

const preciosMock: Record<string, number> = {
  '1': 1200, '2': 900, '3': 1500, '4': 600, '5': 1100,
  '6': 7500, '7': 120, '8': 300, '9': 800, '10': 700, '11': 4500
};

// 🔥 FIX: Actualizado con objetos UnidadMedida e IDs numéricos
let dbProveedores: Proveedor[] = [
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
          // ✅ FIX: UnidadMedida ahora es el objeto {id, nombre}
          { id: 6, nombre: 'Bife de Chorizo', cantidad: 10, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 18450 },
          { id: 5, nombre: 'Tomate Perita', cantidad: 12, unidadMedida: { id: 1, nombre: 'KG' }, precioUnitario: 0 }
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
    historialPedidos: []
  },
  {
    id: 3,
    nombre: 'La Bodega Mayorista',
    contacto: 'Sofía Herrera',
    telefono: '+54 11 3333-9911',
    email: 'contacto@labodegamayorista.com',
    direccion: 'Parque Industrial, Pilar',
    activo: true,
    fechaUltimoPedido: '2026-05-20T11:10:00.000Z',
    categorias: ['Almacen'],
    historialPedidos: []
  },
  {
    id: 4,
    nombre: 'Frutas del Norte',
    contacto: 'Carla Benítez',
    telefono: '+54 11 2222-7744',
    email: 'pedidos@frutasdelnorte.com',
    direccion: 'Mercado Central, La Matanza',
    activo: false,
    fechaUltimoPedido: '2026-04-30T07:40:00.000Z',
    categorias: ['Verdura'],
    historialPedidos: []
  }
];

export const handleProveedorMock = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<any> => {
  const method = req.method;
  const url = req.url;

  if (method === 'GET') {
    if (url.includes('/productos-disponibles')) {
      return of(new HttpResponse({ status: 200, body: INSUMOS_MOCK })).pipe(delay(200));
    }
    
    if (url.includes('/pedido-sugerido-ia')) {
      const parts = url.split('/');
      const idx = parts.findIndex(p => p.toLowerCase() === 'proveedores' || p === 'Proveedor');
      const id = parseInt(parts[idx + 1], 10);
      const prov = dbProveedores.find(p => p.id === id);
      if (!prov) return of(new HttpResponse({ status: 404 }));

      const providerCats = prov.categorias ?? [];
      const sugeridos = INSUMOS_MOCK
        .filter((prod: Insumo) => providerCats.includes(prod.categoriaIngrediente.descripcion))
        .filter((prod: Insumo) => prod.stockActual < prod.stockMinimo * 1.5)
        .map((prod: Insumo) => {
          return {
            productoId: prod.id.toString(),
            nombre: prod.nombre,
            unidadMedida: prod.unidadMedida, // ✅ FIX: Ahora pasa el objeto directamente
            stockActual: prod.stockActual,
            stockMinimo: prod.stockMinimo,
            consumoEstimado30Dias: prod.stockMinimo * 3,
            kind: 'sugerencia',
            cantidadSugerida: Math.max(1, Math.round(prod.stockMinimo * 2 - prod.stockActual)),
            precioUnitario: preciosMock[prod.id.toString()] ?? 500
          };
        });
      return of(new HttpResponse({ status: 200, body: sugeridos })).pipe(delay(200));
    }

    if (url.includes('/historial-pedidos')) {
      const parts = url.split('/');
      const idx = parts.findIndex(p => p.toLowerCase() === 'proveedor' || p.toLowerCase() === 'proveedores');
      const id = parseInt(parts[idx + 1], 10);
      const prov = dbProveedores.find(p => p.id === id);
      if (!prov) return of(new HttpResponse({ status: 404 }));
      return of(new HttpResponse({ status: 200, body: prov.historialPedidos ?? [] })).pipe(delay(200));
    }

    const parts = url.split('/');
    const lastPart = parts.pop() || '';
    const id = parseInt(lastPart, 10);
    if (!isNaN(id)) {
      const prov = dbProveedores.find(p => p.id === id);
      if (prov) {
        const { historialPedidos: _omit, ...proveedorSinHistorial } = prov;
        return of(new HttpResponse({ status: 200, body: proveedorSinHistorial })).pipe(delay(200));
      }
      return of(new HttpResponse({ status: 404 })).pipe(delay(200));
    }

    const sinHistorial = dbProveedores.map(({ historialPedidos: _omit, ...prov }) => prov);
    return of(new HttpResponse({ status: 200, body: sinHistorial })).pipe(delay(200));
  }

  if (method === 'POST') {
    if (url.includes('/confirmar')) {
      const parts = url.split('/');
      const idx = parts.findIndex(p => p.toLowerCase() === 'proveedor' || p.toLowerCase() === 'proveedores');
      const proveedorId = parseInt(parts[idx + 1], 10);
      const pedidoId = parts[idx + 3];
      const index = dbProveedores.findIndex(p => p.id === proveedorId);

      if (index !== -1) {
        const historialPedidos = (dbProveedores[index].historialPedidos ?? []).map(pedido =>
          pedido.id.toString() === pedidoId ? { ...pedido, estado: 'Confirmado' as const } : pedido
        );

        dbProveedores[index] = {
          ...dbProveedores[index],
          historialPedidos
        };

        return of(new HttpResponse({ status: 200, body: historialPedidos })).pipe(delay(200));
      }

      return of(new HttpResponse({ status: 404 }));
    }

    if (url.includes('/items')) {
      const parts = url.split('/');
      const idx = parts.findIndex(p => p.toLowerCase() === 'proveedor' || p.toLowerCase() === 'proveedores');
      const proveedorId = parseInt(parts[idx + 1], 10);
      const pedidoId = parts[idx + 3];
      const index = dbProveedores.findIndex(p => p.id === proveedorId);

      if (index !== -1) {
        const body = req.body as any;
        const item = {
          ...body,
          precioUnitario: body.precioUnitario ?? preciosMock[body.id?.toString()] ?? 500
        };

        const historialPedidos = (dbProveedores[index].historialPedidos ?? []).map(pedido => {
          if (pedido.id.toString() !== pedidoId || pedido.estado !== 'Pendiente') {
            return pedido;
          }

          const itemExistente = pedido.items.find(pedidoItem => pedidoItem.id.toString() === item.id.toString());
          const items = itemExistente
            ? pedido.items.map(pedidoItem => pedidoItem.id.toString() === item.id.toString()
              ? { ...pedidoItem, cantidad: pedidoItem.cantidad + item.cantidad, precioUnitario: item.precioUnitario }
              : pedidoItem)
            : [...pedido.items, item];
          const monto = items.reduce((total, pedidoItem) => total + ((pedidoItem.precioUnitario ?? preciosMock[pedidoItem.id.toString()] ?? 500) * pedidoItem.cantidad), 0);

          return {
            ...pedido,
            items,
            monto
          };
        });

        dbProveedores[index] = {
          ...dbProveedores[index],
          historialPedidos
        };

        return of(new HttpResponse({ status: 200, body: historialPedidos })).pipe(delay(200));
      }

      return of(new HttpResponse({ status: 404 }));
    }

    if (url.includes('/pedidos')) {
      const parts = url.split('/');
      const idx = parts.findIndex(p => p.toLowerCase() === 'proveedor' || p.toLowerCase() === 'proveedores');
      const id = parseInt(parts[idx + 1], 10);
      const index = dbProveedores.findIndex(p => p.id === id);
      if (index !== -1) {
        const body = req.body as NuevoPedidoProveedor;
        const fechaPedido = new Date().toISOString();
        const monto = body.items.reduce((total, item) => total + ((item.precioUnitario ?? 0) * item.cantidad), 0);
        const nuevoPedido: PedidoProveedor = {
          id: Date.now(),
          fecha: fechaPedido,
          concepto: body.concepto,
          monto: body.monto || monto,
          estado: 'Pendiente',
          observacion: body.observacion,
          
          items: body.items.map(item => ({
             id: Number(item.id),
             nombre: item.nombre ?? 'Insumo',
             cantidad: item.cantidad,
             unidadMedida: item.unidadMedida ?? { id: 0, nombre: 'UN' },
             precioUnitario: item.precioUnitario ?? 0
          }))
        };
        dbProveedores[index] = {
          ...dbProveedores[index],
          fechaUltimoPedido: fechaPedido,
          historialPedidos: [nuevoPedido, ...(dbProveedores[index].historialPedidos ?? [])]
        };
        return of(new HttpResponse({ status: 200, body: dbProveedores[index] })).pipe(delay(200));
      }
      return of(new HttpResponse({ status: 404 }));
    }

    const body = req.body as NuevoProveedor;
    const direccion = [body.calle, body.numero].filter(Boolean).join(' ').trim();
    const direccionCompleta = direccion + (body.ciudad ? `, ${body.ciudad}` : '');
    const nuevo: Proveedor = {
      id: Date.now(),
      nombre: body.nombre,
      contacto: body.contacto,
      telefono: body.telefono,
      email: body.email,
      direccion: direccionCompleta,
      activo: true,
      fechaUltimoPedido: null,
      historialPedidos: [],
      categorias: body.categorias ?? []
    };
    dbProveedores.push(nuevo);
    return of(new HttpResponse({ status: 201, body: nuevo })).pipe(delay(200));
  }

  return next(req);
};