import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { ApiService } from '../../../../core/services/api-service';

export interface MedioPagoCaja {
  id: string;
  nombre: string;
  icono: string;
  esperado: number;
  operaciones: number;
  detalle: string;
  tono: 'cash' | 'card' | 'transfer' | 'wallet';
}

export type TurnoCajaId = 'dia' | 'noche';

export interface TurnoCaja {
  id: TurnoCajaId;
  nombre: string;
  horario: string;
  esNocturno: boolean;
}

export interface RankingCajaItem {
  nombre: string;
  valor: number;
  detalle: string;
}

export interface EncuestaCajaItem {
  label: string;
  valor: number;
  detalle: string;
}

export interface HistorialCierreItem {
  fecha: string;
  turno: string;
  total: number;
  diferencia: number;
  estado: 'Cuadrada' | 'Sobrante' | 'Faltante';
}

export interface PasoCierreCaja {
  id: string;
  label: string;
  detalle: string;
  icono: string;
}

const TURNOS: TurnoCaja[] = [
  { id: 'dia', nombre: 'Turno Dia', horario: '08:00 a 16:00', esNocturno: false },
  { id: 'noche', nombre: 'Turno Noche', horario: '16:00 a 00:00', esNocturno: true }
];

const PASOS_CIERRE: PasoCierreCaja[] = [
  { id: 'turno', label: 'Turno', detalle: 'Seleccionado', icono: 'schedule' },
  { id: 'conciliacion', label: 'Conciliacion', detalle: 'Efectivo contado', icono: 'point_of_sale' },
  { id: 'confirmacion', label: 'Confirmacion', detalle: 'Listo para cerrar', icono: 'task_alt' }
];

@Injectable({ providedIn: 'root' })
export class CierreCajaStateService {
  private api = inject(ApiService);

  // Señales de estado local editables por el usuario
  private readonly turnoSeleccionadoSignal = signal<TurnoCajaId>('noche');
  private readonly efectivoContadoSignal = signal(0);
  private readonly observacionSignal = signal('');
  private readonly cierreConfirmadoSignal = signal(false);

  // Señales de datos cargados de forma asíncrona desde el backend
  private readonly loadingSignal = signal(false);
  private readonly platosMasVendidosSignal = signal<RankingCajaItem[]>([]);
  private readonly platosMenosVendidosSignal = signal<RankingCajaItem[]>([]);
  private readonly insumosMasUsadosSignal = signal<RankingCajaItem[]>([]);
  private readonly historialCierresSignal = signal<HistorialCierreItem[]>([]);
  private readonly mediosPagoSignal = signal<MedioPagoCaja[]>([]);

  readonly fechaTurno = '10/06/2026';
  readonly gerente = 'Carlos Lopez';
  readonly turnos = TURNOS;
  readonly pasosCierre = PASOS_CIERRE;
  readonly propinasProximamente = true;
  readonly encuestasProximamente = true;

  // Exposición de señales públicas de solo lectura
  readonly loading = this.loadingSignal.asReadonly();
  readonly turnoSeleccionadoId = this.turnoSeleccionadoSignal.asReadonly();
  readonly turnoSeleccionado = computed(() => this.turnos.find(turno => turno.id === this.turnoSeleccionadoId()) ?? this.turnos[0]);
  readonly mediosPago = this.mediosPagoSignal.asReadonly();
  readonly observacion = this.observacionSignal.asReadonly();
  readonly cierreConfirmado = this.cierreConfirmadoSignal.asReadonly();
  readonly efectivoContado = this.efectivoContadoSignal.asReadonly();

  // Señales calculadas
  readonly efectivoEsperado = computed(() => this.mediosPago().find(medio => medio.id === 'efectivo')?.esperado ?? 0);
  readonly diferenciaEfectivo = computed(() => this.efectivoContado() - this.efectivoEsperado());
  readonly totalRecaudado = computed(() => this.mediosPago().reduce((total, medio) => total + medio.esperado, 0));
  readonly totalOperaciones = computed(() => this.mediosPago().reduce((total, medio) => total + medio.operaciones, 0));
  readonly requiereObservacion = computed(() => this.diferenciaEfectivo() !== 0);
  readonly puedeConfirmar = computed(() => !this.cierreConfirmado() && (!this.requiereObservacion() || this.observacion().trim().length >= 8));
  
  readonly estadoCaja = computed(() => {
    const diferencia = this.diferenciaEfectivo();
    if (diferencia > 0) return 'Sobrante';
    if (diferencia < 0) return 'Faltante';
    return 'Cuadrada';
  });

  // Getters para exponer los arreglos como propiedades simples y mantener compatibilidad con el HTML
  get platosMasVendidos(): RankingCajaItem[] {
    return this.platosMasVendidosSignal();
  }

  get platosMenosVendidos(): RankingCajaItem[] {
    return this.platosMenosVendidosSignal();
  }

  get insumosMasUsados(): RankingCajaItem[] {
    return this.insumosMasUsadosSignal();
  }

  get historialCierres(): HistorialCierreItem[] {
    return this.historialCierresSignal();
  }

  // Elementos adicionales del resumen y encuestas requeridos por el HTML
  readonly resumenTurno = {
    comandasProcesadas: 259,
    platosRealizados: 216,
    comensales: 184,
    tiempoPromedioComandas: '18 min',
    encuestasRespondidas: 67,
    satisfaccionPromedio: 88
  };

  readonly encuestas: EncuestaCajaItem[] = [
    { label: 'Atencion', valor: 92, detalle: 'muy buena o excelente' },
    { label: 'Comida', valor: 88, detalle: 'satisfechos' },
    { label: 'Tiempo de espera', valor: 79, detalle: 'lo percibio correcto' },
    { label: 'Volveria', valor: 94, detalle: 'probabilidad alta' }
  ];

  readonly propinas = computed(() => 0);

  readonly confirmacionMensaje = computed(() => {
    if (this.cierreConfirmado()) return 'Cierre confirmado. La informacion queda en modo lectura.';
    if (this.requiereObservacion() && this.observacion().trim().length < 8) {
      return 'Ingresa una observacion de al menos 8 caracteres para confirmar la diferencia.';
    }
    return 'Listo para confirmar el cierre del turno.';
  });

  readonly confirmacionLabel = computed(() => this.cierreConfirmado() ? 'Cierre confirmado' : 'Confirmar cierre');

  readonly pasoActual = computed(() => {
    if (this.cierreConfirmado()) return 'confirmacion';
    if (this.efectivoContado() !== this.efectivoEsperado() || this.observacion().trim()) return 'conciliacion';
    return 'turno';
  });

  constructor() {
    this.cargarDatosTurno();
    this.cargarHistorial();
  }

  cargarDatosTurno(): void {
    const turnoIdVal = this.turnoSeleccionadoId() === 'dia' ? 1 : 2;
    this.loadingSignal.set(true);

    const params = new HttpParams()
      .set('fecha', '2026-06-10')
      .set('turnoLaboralId', turnoIdVal.toString());

    this.api.get<any>('cierre/turno', params).subscribe({
      next: (data) => {
        if (data) {
          // Desglose de medios de pago
          this.mediosPagoSignal.set(data.desglosePagos.map((medio: any) => ({
            id: medio.nombre.toLowerCase().replace(/\s+/g, '-'),
            nombre: medio.nombre,
            icono: medio.nombre.toLowerCase().includes('efectivo') ? 'payments' 
                 : medio.nombre.toLowerCase().includes('tarjeta') ? 'credit_card' 
                 : medio.nombre.toLowerCase().includes('transferencia') ? 'sync_alt' 
                 : 'account_balance_wallet',
            esperado: medio.esperado,
            operaciones: medio.operaciones,
            detalle: `Cobrado vía ${medio.nombre}`,
            tono: medio.nombre.toLowerCase().includes('efectivo') ? 'cash' 
                : medio.nombre.toLowerCase().includes('tarjeta') ? 'card' 
                : medio.nombre.toLowerCase().includes('transferencia') ? 'transfer' 
                : 'wallet'
          })));

          // Rendimiento de platos e insumos
          this.platosMasVendidosSignal.set(data.rendimientoTurno.platosMasVendidos.map((plato: any) => ({
            nombre: plato.nombre,
            valor: plato.cantidad,
            detalle: `$ ${plato.total.toLocaleString('es-AR')}`
          })));

          this.insumosMasUsadosSignal.set(data.rendimientoTurno.insumosMasUsados.map((insumo: any) => ({
            nombre: insumo.nombre,
            valor: insumo.cantidad,
            detalle: `${insumo.unidad} usados`
          })));

          // Resetear el efectivo contado al valor esperado inicialmente
          this.efectivoContadoSignal.set(this.efectivoEsperado());
        }
        this.loadingSignal.set(false);
      },
      error: () => {
        this.loadingSignal.set(false);
      }
    });
  }

  cargarHistorial(): void {
    this.api.get<any[]>('cierre/historial').subscribe({
      next: (data) => {
        if (data) {
          this.historialCierresSignal.set(data.map((c: any) => ({
            fecha: c.fecha,
            turno: c.turno,
            total: c.total,
            diferencia: c.diferencia,
            estado: c.estado
          })));
        }
      },
      error: () => {}
    });
  }

  seleccionarTurno(turnoId: TurnoCajaId): void {
    this.turnoSeleccionadoSignal.set(turnoId);
    this.observacionSignal.set('');
    this.cierreConfirmadoSignal.set(false);
    this.cargarDatosTurno();
  }

  setEfectivoContado(value: string | number): void {
    if (this.cierreConfirmado()) return;
    const normalized = Number(String(value).replace(/[^\d.-]/g, ''));
    this.efectivoContadoSignal.set(Number.isFinite(normalized) ? Math.max(0, normalized) : 0);
  }

  setObservacion(value: string): void {
    this.observacionSignal.set(value);
  }

  confirmarCierre(): void {
    if (!this.puedeConfirmar()) return;

    const body = {
      restauranteId: 1,
      turnoLaboralId: this.turnoSeleccionadoId() === 'dia' ? 1 : 2,
      efectivoContado: this.efectivoContado(),
      diferencia: this.diferenciaEfectivo(),
      sobrante: this.diferenciaEfectivo() > 0 ? this.diferenciaEfectivo() : 0,
      observacion: this.observacion()
    };

    this.api.post('cierre', body).subscribe({
      next: () => {
        this.cierreConfirmadoSignal.set(true);
        this.cargarHistorial();
      }
    });
  }
}
