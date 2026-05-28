import { HttpRequest, HttpResponse, HttpHandlerFn } from "@angular/common/http";
import { delay, Observable, of } from "rxjs";
import { PRODUCTOS_STOCK_MOCK, ProductoStock } from '../../model/producto-stock';

// El estado de memoria queda encapsulado solo para este dominio
let dbMemoria: ProductoStock[] = [...PRODUCTOS_STOCK_MOCK];

export const handleStockMock = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<any> => {
  const method = req.method;
  const url = req.url;

  if (method === 'GET') {
    return of(new HttpResponse({ status: 200, body: [...dbMemoria] })).pipe(delay(600));
  }

  if (method === 'POST') {
    const nuevoProducto = req.body as ProductoStock;
    nuevoProducto.id = dbMemoria.length > 0 ? Math.max(...dbMemoria.map(p => p.id)) + 1 : 1;
    dbMemoria.push(nuevoProducto);
    return of(new HttpResponse({ status: 201, body: nuevoProducto })).pipe(delay(600));
  }

  if (method === 'PUT') {
    const id = parseInt(url.split('/').pop() || '0', 10);
    const index = dbMemoria.findIndex(p => p.id === id);
    if (index !== -1) {
      dbMemoria[index] = { ...dbMemoria[index], ...(req.body as any) };
      return of(new HttpResponse({ status: 200, body: dbMemoria[index] })).pipe(delay(600));
    }
  }

  if (method === 'DELETE') {
    const id = parseInt(url.split('/').pop() || '0', 10);
    dbMemoria = dbMemoria.filter(p => p.id !== id);
    return of(new HttpResponse({ status: 204 })).pipe(delay(600));
  }

  // Fallback de seguridad
  return next(req);
};
