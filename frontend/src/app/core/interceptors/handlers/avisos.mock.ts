import { HttpRequest, HttpHandlerFn, HttpResponse } from '@angular/common/http';
import { of } from 'rxjs';
import { AvisosResponseDto } from '../../models/aviso.model';

export function handleAvisosMock(req: HttpRequest<any>, next: HttpHandlerFn) {
  if (req.method === 'GET' && req.url.includes('/avisos')) {
    const mockResponse: AvisosResponseDto = {
      insumosConStockCritico: [
        {
          id: 1,
          nombre: 'Harina 0000',
          stockActual: 1.5,
          unidadMedida: 'Kg',
          stockMinimo: 15,
          estadoStock: 'Crítico',
          tipo: 'Ingrediente',
          vencimiento: null,
          categoria: 'Secos'
        },
        {
          id: 2,
          nombre: 'Aceite de Girasol',
          stockActual: 2,
          unidadMedida: 'L',
          stockMinimo: 5,
          estadoStock: 'Crítico',
          tipo: 'Ingrediente',
          vencimiento: null,
          categoria: 'Líquidos'
        }
      ],
      insumosConVencimientoProximo: {
        '3': [
          {
            id: 101,
            nombre: 'Leche - Lote 4B',
            insumoId: 3,
            cantidad: 5,
            fechaVencimiento: '2026-06-05',
            bodegaId: 1
          }
        ],
        '4': [
          {
            id: 102,
            nombre: 'Queso Cremoso - Lote 9A',
            insumoId: 4,
            cantidad: 2.5,
            fechaVencimiento: '2026-06-03',
            bodegaId: 1
          }
        ]
      }
    };

    return of(new HttpResponse({ status: 200, body: mockResponse }));
  }

  if (req.method === 'GET' && req.url.endsWith('/Insumo')) {
    const mockInsumos = [
      {
        id: 101,
        nombre: 'Leche Larga Vida',
        stockActual: 5,
        unidadMedida: 'Lt',
        vencimiento: '2026-06-05',
        stockMinimo: 2,
        categoria: 'Lácteos'
      },
      {
        id: 102,
        nombre: 'Queso Cremoso',
        stockActual: 2.5,
        unidadMedida: 'Kg',
        vencimiento: '2026-06-03',
        stockMinimo: 5,
        categoria: 'Lácteos'
      }
    ];
    return of(new HttpResponse({ status: 200, body: mockInsumos }));
  }

  return next(req);
}
