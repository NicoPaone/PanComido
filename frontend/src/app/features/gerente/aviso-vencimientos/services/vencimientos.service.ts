import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { IngredienteVencimiento, VencimientoProveedor, VencimientoPedidoActivo } from '../../../../core/models/vencimientos.model';

@Injectable({ providedIn: 'root' })
export class VencimientosApiService {

  // Mock data
  private ingredientesMock: IngredienteVencimiento[] = [
    { id: '1', nombre: 'Leche Entera', fechaVencimiento: '2026-06-02', stockDisponible: 5, unidadMedida: 'L' },
    { id: '2', nombre: 'Aceite de Girasol', fechaVencimiento: '2026-06-05', stockDisponible: 12, unidadMedida: 'L' },
    { id: '3', nombre: 'Harina 0000', fechaVencimiento: '2026-06-10', stockDisponible: 50, unidadMedida: 'Kg' }
  ];

  private proveedoresMock: Record<string, VencimientoProveedor[]> = {
    '1': [{ id: 'p1', nombre: 'Lácteos Sur' }, { id: 'p2', nombre: 'Distribuidora Central' }],
    '2': [{ id: 'p2', nombre: 'Distribuidora Central' }, { id: 'p3', nombre: 'Aceites y Cia' }],
    '3': [{ id: 'p4', nombre: 'Molinos del Sol' }]
  };

  private pedidosMock: Record<string, VencimientoPedidoActivo[]> = {
    'p1': [{ id: 'ped1', numeroEnvio: 'ENV-001', fechaCreacion: '2026-05-25' }],
    'p2': [{ id: 'ped2', numeroEnvio: 'ENV-099', fechaCreacion: '2026-05-27' }, { id: 'ped3', numeroEnvio: 'ENV-104', fechaCreacion: '2026-05-28' }],
    'p3': [],
    'p4': [{ id: 'ped4', numeroEnvio: 'ENV-202', fechaCreacion: '2026-05-26' }]
  };

  getIngredientesProximosVencer(): Observable<IngredienteVencimiento[]> {
    return of(this.ingredientesMock).pipe(delay(500));
  }

  getProveedoresPorIngrediente(ingredienteId: string): Observable<VencimientoProveedor[]> {
    return of(this.proveedoresMock[ingredienteId] || []).pipe(delay(400));
  }

  getPedidosActivosPorProveedor(proveedorId: string): Observable<VencimientoPedidoActivo[]> {
    return of(this.pedidosMock[proveedorId] || []).pipe(delay(400));
  }

  agregarAPedidoExistente(pedidoId: string, ingredienteId: string, cantidad: number): Observable<boolean> {
    console.log(`Mock request: Agregar ${cantidad} del ing ${ingredienteId} al pedido ${pedidoId}`);
    return of(true).pipe(delay(600));
  }
}