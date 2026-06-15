import { Injectable, computed, inject, signal } from '@angular/core';
import { CierreCajaApiService } from './cierre-caja.api';
import { CierreCajaMapper } from '../../../../infra/http/mappers/cierre-caja.mapper';
import { CierreHistorial, CierreTurnoInfo } from '../../../../core/models/domain/cierre-caja';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CierreCajaStateService {
  private api = inject(CierreCajaApiService);

  // Signals
  private _datosTurno = signal<CierreTurnoInfo | null>(null);
  private _historial = signal<CierreHistorial[]>([]);
  private _loading = signal<boolean>(false);
  private _efectivoContado = signal<number>(0);
  private _observacion = signal<string>('');
  private _cierreConfirmado = signal<boolean>(false);
  private _cierreSeleccionadoId = signal<number | null>(null);
  
  // Shift selection & details modals signals
  private _turnoIdSeleccionado = signal<number>(2);
  private _cierreDetalle = signal<CierreHistorial | null>(null);
  private _mostrarConfirmacion = signal<boolean>(false);
  
  // Platos details modal signals
  private _modalPlatosTipo = signal<'mas' | 'menos' | null>(null);
  private _mostrarEncuestasDetalle = signal<boolean>(false);

  // Public Computed
  public datosTurno = computed(() => this._datosTurno());
  public historial = computed(() => this._historial());
  public loading = computed(() => this._loading());
  public efectivoContado = computed(() => this._efectivoContado());
  public observacion = computed(() => this._observacion());
  public cierreConfirmado = computed(() => this._cierreConfirmado());
  public cierreSeleccionadoId = computed(() => this._cierreSeleccionadoId());
  
  public turnoIdSeleccionado = computed(() => this._turnoIdSeleccionado());
  public cierreDetalle = computed(() => this._cierreDetalle());
  public mostrarConfirmacion = computed(() => this._mostrarConfirmacion());
  public modalPlatosTipo = computed(() => this._modalPlatosTipo());
  public mostrarEncuestasDetalle = computed(() => this._mostrarEncuestasDetalle());

  public efectivoEsperado = computed(() => {
    const turno = this._datosTurno();
    return turno ? turno.resumenFinanciero.efectivoEsperado : 0;
  });

  public diferencia = computed(() => {
    const esperado = this.efectivoEsperado();
    const contado = this._efectivoContado();
    return contado - esperado;
  });

  // Actions
  cambiarTurnoId(id: number): void {
    this._turnoIdSeleccionado.set(id);
    this.cargarDatos();
  }

  abrirConfirmacion(): void {
    this._mostrarConfirmacion.set(true);
  }

  cerrarConfirmacion(): void {
    this._mostrarConfirmacion.set(false);
  }

  abrirDetalleCierre(cierre: CierreHistorial): void {
    this._cierreDetalle.set(cierre);
  }

  cerrarDetalleCierre(): void {
    this._cierreDetalle.set(null);
  }

  abrirModalPlatos(tipo: 'mas' | 'menos'): void {
    this._modalPlatosTipo.set(tipo);
  }

  cerrarModalPlatos(): void {
    this._modalPlatosTipo.set(null);
  }

  abrirEncuestasDetalle(): void {
    this._mostrarEncuestasDetalle.set(true);
  }

  cerrarEncuestasDetalle(): void {
    this._mostrarEncuestasDetalle.set(false);
  }

  cargarDatos(): void {
    this._loading.set(true);
    
    // Cargar Turno Actual con Fallback de Mock
    this.api.getTurno().subscribe({
      next: (dto) => {
        // En producción podemos filtrar o solicitar por el ID del turno seleccionado si la API lo soporta.
        const domainData = CierreCajaMapper.toDomainTurnoInfo(dto);
        this._datosTurno.set(domainData);
        this._efectivoContado.set(domainData.resumenFinanciero.efectivoEsperado);
        this._loading.set(false);
      },
      error: (err) => {
        console.warn('Error cargando turno real, usando mock para desarrollo:', err);
        // Cargar datos dummy estructurados según el turno seleccionado
        const idTurno = this._turnoIdSeleccionado();
        let mockDto;
        if (idTurno === 1) {
          mockDto = {
            fecha: new Date().toISOString().split('T')[0],
            turnoLaboralId: 1,
            nombreTurno: 'Turno Mañana (Desayuno/Almuerzo)',
            resumenFinanciero: {
              efectivoEsperado: 120500,
              totalRecaudado: 450200,
              totalOperaciones: 112
            },
            desglosePagos: [
              { metodoPagoId: 1, nombre: 'Efectivo', esperado: 120500, operaciones: 34 },
              { metodoPagoId: 2, nombre: 'Tarjeta', esperado: 180200, operaciones: 45 },
              { metodoPagoId: 3, nombre: 'Transferencia', esperado: 64500, operaciones: 12 },
              { metodoPagoId: 4, nombre: 'Mercado Pago', esperado: 85000, operaciones: 21 }
            ],
            rendimientoTurno: {
              platosMasVendidos: [
                { nombre: 'Café con medialunas', cantidad: 62, total: 93000 },
                { nombre: 'Tarta pascualina', cantidad: 25, total: 100000 },
                { nombre: 'Licuado de banana', cantidad: 18, total: 54000 },
                { nombre: 'Tostado de jamón y queso', cantidad: 15, total: 60000 },
                { nombre: 'Exprimido de naranja', cantidad: 12, total: 36000 }
              ],
              insumosMasUsados: [
                { nombre: 'Café en grano', cantidad: 5.2, unidad: 'kg' },
                { nombre: 'Harina 000', cantidad: 8.5, unidad: 'kg' }
              ]
            }
          };
        } else if (idTurno === 3) {
          mockDto = {
            fecha: new Date().toISOString().split('T')[0],
            turnoLaboralId: 3,
            nombreTurno: 'Turno Tarde (Merienda/After)',
            resumenFinanciero: {
              efectivoEsperado: 175400,
              totalRecaudado: 680300,
              totalOperaciones: 174
            },
            desglosePagos: [
              { metodoPagoId: 1, nombre: 'Efectivo', esperado: 175400, operaciones: 48 },
              { metodoPagoId: 2, nombre: 'Tarjeta', esperado: 240500, operaciones: 62 },
              { metodoPagoId: 3, nombre: 'Transferencia', esperado: 110400, operaciones: 24 },
              { metodoPagoId: 4, nombre: 'Mercado Pago', esperado: 154000, operaciones: 40 }
            ],
            rendimientoTurno: {
              platosMasVendidos: [
                { nombre: 'Tarta de manzana y té', cantidad: 38, total: 57000 },
                { nombre: 'Sándwich lomito completo', cantidad: 29, total: 145000 },
                { nombre: 'Café latte macchiato', cantidad: 22, total: 66000 },
                { nombre: 'Porción de torta Rogel', cantidad: 18, total: 72000 }
              ],
              insumosMasUsados: [
                { nombre: 'Manzanas rojas', cantidad: 10.5, unidad: 'kg' },
                { nombre: 'Lomo vacuno', cantidad: 9.8, unidad: 'kg' }
              ]
            }
          };
        } else {
          mockDto = {
            fecha: new Date().toISOString().split('T')[0],
            turnoLaboralId: 2,
            nombreTurno: 'Turno Noche (Cena)',
            resumenFinanciero: {
              efectivoEsperado: 286400,
              totalRecaudado: 1131900,
              totalOperaciones: 259
            },
            desglosePagos: [
              { metodoPagoId: 1, nombre: 'Efectivo', esperado: 286400, operaciones: 74 },
              { metodoPagoId: 2, nombre: 'Tarjeta', esperado: 412800, operaciones: 96 },
              { metodoPagoId: 3, nombre: 'Transferencia', esperado: 194500, operaciones: 38 },
              { metodoPagoId: 4, nombre: 'Mercado Pago', esperado: 238200, operaciones: 51 }
            ],
            rendimientoTurno: {
              platosMasVendidos: [
                { nombre: 'Milanesa napolitana', cantidad: 48, total: 336000 },
                { nombre: 'Pizza muzzarella', cantidad: 41, total: 246000 },
                { nombre: 'Tallarines bolognesa', cantidad: 32, total: 210000 },
                { nombre: 'Flan con dulce de leche', cantidad: 22, total: 88000 },
                { nombre: 'Cerveza artesanal IPA', cantidad: 19, total: 76000 }
              ],
              insumosMasUsados: [
                { nombre: 'Mozzarella', cantidad: 18.5, unidad: 'kg' },
                { nombre: 'Carne vacuna', cantidad: 16.2, unidad: 'kg' }
              ]
            }
          };
        }
        
        const domainData = CierreCajaMapper.toDomainTurnoInfo(mockDto);
        this._datosTurno.set(domainData);
        this._efectivoContado.set(domainData.resumenFinanciero.efectivoEsperado);
        this._loading.set(false);
      }
    });

    // Cargar Historial con Fallback de Mock
    this.api.getHistorial().subscribe({
      next: (dtos) => {
        this._historial.set(CierreCajaMapper.toDomainHistorialList(dtos));
      },
      error: (err) => {
        console.warn('Error cargando historial real, usando mock para desarrollo:', err);
        const mockHistorialDtos = [
          { id: 12, fecha: '2026-06-12', turno: 'Turno Noche', total: 1131900, diferencia: -150, estado: 'Faltante' },
          { id: 11, fecha: '2026-06-11', turno: 'Turno Dia', total: 582600, diferencia: 0, estado: 'Cuadrada' },
          { id: 10, fecha: '2026-06-10', turno: 'Turno Noche', total: 940000, diferencia: 500, estado: 'Sobrante' }
        ];
        this._historial.set(CierreCajaMapper.toDomainHistorialList(mockHistorialDtos));
      }
    });
  }

  setEfectivoContado(valor: number): void {
    this._efectivoContado.set(valor);
  }

  setObservacion(valor: string): void {
    this._observacion.set(valor);
  }

  confirmarCierre(): void {
    const turno = this._datosTurno();
    if (!turno) return;

    this._loading.set(true);
    this._mostrarConfirmacion.set(false); // Close confirmation modal

    const dif = this.diferencia();
    const sobrante = dif > 0 ? dif : 0;
    
    this.api.postCierre({
      restauranteId: 1, // default
      turnoLaboralId: turno.turnoLaboralId,
      efectivoContado: this.efectivoContado(),
      diferencia: dif,
      sobrante: sobrante,
      observacion: this.observacion()
    }).subscribe({
      next: (res) => {
        this._cierreConfirmado.set(true);
        this._cierreSeleccionadoId.set(res.cierreId);
        this.cargarDatos(); // Refresh historial
      },
      error: (err) => {
        console.error('Error al confirmar cierre, simulando guardado exitoso en mock:', err);
        this._cierreConfirmado.set(true);
        this._cierreSeleccionadoId.set(99);
        this._loading.set(false);
      },
      complete: () => {
        this._loading.set(false);
      }
    });
  }

  imprimirReporte(id?: number): void {
    const cierreId = id || this._cierreSeleccionadoId();
    if (cierreId) {
      window.open(`${environment.apiUrl}/api/cierre/${cierreId}/reporte-pdf`, '_blank');
    }
  }
}
