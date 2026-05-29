import { HttpRequest, HttpResponse, HttpHandlerFn } from "@angular/common/http";
import { delay, Observable, of } from "rxjs";
import { Insumo, INSUMOS_MOCK } from '../../models/producto-stock';

// El estado de memoria queda encapsulado solo para este dominio
let dbMemoria: Insumo[] = [...INSUMOS_MOCK];

export const handleStockMock = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<any> => {
  const method = req.method;
  const url = req.url;

  if (method === 'GET') {
    return of(new HttpResponse({ status: 200, body: [...dbMemoria] })).pipe(delay(600));
  }

  if (method === 'POST') {
    const nuevoInsumo = req.body as Insumo;
    nuevoInsumo.id = dbMemoria.length > 0 ? Math.max(...dbMemoria.map(p => p.id)) + 1 : 1;
    dbMemoria.push(nuevoInsumo);
    return of(new HttpResponse({ status: 201, body: nuevoInsumo })).pipe(delay(600));
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

  // Fallback de seguridad (no debería llegar aquí, pero TS lo exige)
  return next(req);
};
