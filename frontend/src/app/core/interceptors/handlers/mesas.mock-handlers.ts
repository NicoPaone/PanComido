import { HttpRequest, HttpResponse, HttpHandlerFn } from '@angular/common/http';
import { Observable, delay, of } from 'rxjs';
import { Mesa, EstadoMesa, FormaMesa } from '../../models/mesa.model';

// Base de datos en RAM inicializada con 3 mesas de prueba
let dbMesas: Mesa[] = [
  {
    id: 1,
    codigoInvitacion: 'MESA-01',
    numeroMesa: 1,
    cantidadPersonasMax: 4,
    estadoMesa: EstadoMesa.Disponible,
    dimensionMesa: { id: 1, forma: FormaMesa.Cuadrada, imagen: '' },
    posicionXInicio: 50,
    posicionXfin: 150,
    posicionYinicio: 50,
    posicionYFin: 150
  },
  {
    id: 2,
    codigoInvitacion: 'MESA-02',
    numeroMesa: 2,
    cantidadPersonasMax: 2,
    estadoMesa: EstadoMesa.Ocupada,
    dimensionMesa: { id: 2, forma: FormaMesa.Redonda, imagen: '' },
    posicionXInicio: 200,
    posicionXfin: 280,
    posicionYinicio: 50,
    posicionYFin: 130
  },
  {
    id: 3,
    codigoInvitacion: 'MESA-03',
    numeroMesa: 3,
    cantidadPersonasMax: 6,
    estadoMesa: EstadoMesa.Reservada,
    dimensionMesa: { id: 3, forma: FormaMesa.HorizontalLarga, imagen: '' },
    posicionXInicio: 50,
    posicionXfin: 200,
    posicionYinicio: 200,
    posicionYFin: 280
  }
];

export const handleMesasMock = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<any> => {
  const method = req.method;
  const url = req.url;

  // GET: Traer todas las mesas
  if (method === 'GET') {
    return of(new HttpResponse({ status: 200, body: [...dbMesas] })).pipe(delay(400));
  }

  // PUT: Actualizar posición o estado de la mesa (Drag & Drop)
  if (method === 'PUT') {
    const id = parseInt(url.split('/').pop() || '0', 10);
    const index = dbMesas.findIndex(m => m.id === id);

    if (index !== -1) {
      dbMesas[index] = { ...dbMesas[index], ...(req.body as Partial<Mesa>) };
      return of(new HttpResponse({ status: 200, body: dbMesas[index] })).pipe(delay(200));
    }
  }

  return next(req);
};
