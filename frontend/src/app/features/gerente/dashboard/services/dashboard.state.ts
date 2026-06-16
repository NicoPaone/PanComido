import { computed, inject, Injectable, signal } from '@angular/core';
import { 
  DashboardPeriodo, DashboardRankingItem, DashboardInsumoVencimiento, 
  DashboardLecturaComercial, DashboardAtencionItem, DashboardAccionItem, 
  DashboardDestino, DashboardVentaMensual, DashboardVentaDia, DashboardFacturacionCentro 
} from '../../../../core/models/domain/dashboard';
import { DashboardApiService } from './dashboard.api';

const MOCK_ACCIONES: DashboardAccionItem[] = [
  { titulo: 'Crear pedido sugerido', detalle: 'Reponer insumos criticos', destino: 'pedido', tono: 'danger', impacto: 'Evita quiebres', prioridad: 1 },
  { titulo: 'Ver vencimientos', detalle: 'Priorizar consumo y descarte', destino: 'stock', tono: 'warning', impacto: 'Reduce merma', prioridad: 2 },
  { titulo: 'Revisar carta', detalle: 'Ajustar baja demanda', destino: 'carta', tono: 'info', impacto: 'Recupera ventas', prioridad: 3 }
];

@Injectable({ providedIn: 'root' })
export class DashboardStateService {
  private api = inject(DashboardApiService);

  private _periodo = signal<DashboardPeriodo>('7d');
  private _fechaDesde = signal<string>('');
  private _fechaHasta = signal<string>('');

  private _vencimientos = signal<DashboardInsumoVencimiento[]>([]);
  private _platosMasVendidos = signal<DashboardRankingItem[]>([]);
  private _platosMenosVendidos = signal<DashboardRankingItem[]>([]);

  periodo = this._periodo.asReadonly();
  fechaDesde = this._fechaDesde.asReadonly();
  fechaHasta = this._fechaHasta.asReadonly();

  insumosPorVencer = this._vencimientos.asReadonly();
  platosMasVendidos = this._platosMasVendidos.asReadonly();
  platosMenosVendidos = this._platosMenosVendidos.asReadonly();

  vencimientosResumen = computed(() => {
    const insumos = this.insumosPorVencer();
    return [
      { label: 'En radar', value: insumos.length, tone: 'info' },
      { label: 'Alta', value: insumos.filter(item => item.criticidad === 'alta').length, tone: 'danger' },
      { label: 'Media', value: insumos.filter(item => item.criticidad === 'media').length, tone: 'warning' },
      { label: 'Baja', value: insumos.filter(item => item.criticidad === 'baja').length, tone: 'neutral' }
    ];
  });

  recomendacionOperativa = computed(() => {
    const criticos = this.insumosPorVencer().filter(item => item.criticidad === 'alta');
    if (criticos.length === 0) {
      return 'Sin insumos de prioridad alta. Mantener seguimiento de media y baja criticidad.';
    }
    const nombresList = criticos.map(item => item.nombre);
    let nombres = '';
    if (nombresList.length <= 4) {
      nombres = nombresList.length > 1 
        ? nombresList.slice(0, -1).join(', ') + ' y ' + nombresList.slice(-1)
        : nombresList[0];
    } else {
      nombres = nombresList.slice(0, 4).join(', ') + ` y ${nombresList.length - 4} más`;
    }
    return `Priorizar ${nombres} en preparaciones del día para reducir merma y evitar faltantes.`;
  });

  atencion = computed<DashboardAtencionItem[]>(() => {
    const criticos = this.insumosPorVencer().filter(i => i.criticidad === 'alta').length;
    const platosEnBaja = this.platosMenosVendidosPreview().length;

    const items: DashboardAtencionItem[] = [];
    if (criticos > 0) {
      items.push({
        titulo: `${criticos} insumos criticos`,
        detalle: 'Vencen pronto',
        accion: 'Ver stock',
        destino: 'stock',
        tono: 'danger'
      });
    }
    if (platosEnBaja > 0) {
      items.push({
        titulo: `${platosEnBaja} platos en baja`,
        detalle: 'Demanda ha caido',
        accion: 'Revisar carta',
        destino: 'carta',
        tono: 'warning'
      });
    }
    return items;
  });

  acciones = signal<DashboardAccionItem[]>(MOCK_ACCIONES).asReadonly();

  
  tituloGrafico = computed(() => this._periodo() === '30d' ? 'Ventas del mes' : 'Tendencia de ventas');
  subtituloGrafico = computed(() => this._periodo() === '30d' ? 'Mapa de calor por dia' : 'Evolucion del periodo');

  ventasMensuales = computed<DashboardVentaMensual[]>(() => {
    const periodo = this._periodo();
    if (periodo === '1d') return MOCK_VENTAS_24H;
    if (periodo === '3d') return MOCK_VENTAS_3D;
    if (periodo === '7d') return MOCK_VENTAS_7D;
    if (periodo === '30d') return MOCK_VENTAS_30D;
    if (periodo === 'custom') return this.ventasPersonalizadas();
    return MOCK_VENTAS_MENSUALES;
  });

  ventasCalendarioMes = computed<DashboardVentaDia[]>(() => {
    const factor = this._periodo() === '30d' ? 1 : Math.max(0.7, Math.min(1.25, this.factorPeriodo() / 4.2));
    return MOCK_VENTAS_DIAS_MES.map(item => ({
      ...item,
      ventas: Math.round(item.ventas * factor)
    }));
  });

  maxVentasCalendarioMes = computed(() => {
    return Math.max(...this.ventasCalendarioMes().map(item => item.ventas));
  });

  facturacionPorCentro = computed<DashboardFacturacionCentro[]>(() => {
    const factor = this.factorPeriodo();
    return MOCK_FACTURACION_CENTRO.map(centro => ({
      ...centro,
      total: Math.round(centro.total * factor)
    }));
  });

  maxVentasMensuales = computed(() => {
    return Math.max(...this.ventasMensuales().map(item => item.ventas));
  });

  totalFacturacion = computed(() => {
    return this.facturacionPorCentro().reduce((total, centro) => total + centro.total, 0);
  });

  facturacionDonutGradient = computed(() => {
    let acumulado = 0;
    const segmentos = this.facturacionPorCentro().map(centro => {
      const desde = acumulado;
      acumulado += centro.porcentaje;
      return `${centro.color} ${desde}% ${acumulado}%`;
    });

    return `conic-gradient(${segmentos.join(', ')})`;
  });

  lecturaCanales = computed(() => [
    { titulo: 'Mostrador en alza', detalle: '+15% comparado al mes anterior, impulsado por mediodías.' },
    { titulo: 'Delivery estable', detalle: 'Representa el mayor volumen, pero con ticket promedio más bajo.' }
  ]);

  periodoLabel = computed(() => {
    const labels: Record<DashboardPeriodo, string> = {
      '1d': 'Ultimas 24 horas',
      '3d': 'Ultimos 3 dias',
      '7d': 'Ultima semana',
      '30d': 'Ultimo mes',
      '365d': 'Ultimo año',
      custom: this._fechaDesde() && this._fechaHasta()
        ? `${this._fechaDesde()} al ${this._fechaHasta()}`
        : 'Fecha personalizada'
    };
    return labels[this._periodo()];
  });

  platosMasVendidosPreview = computed(() => this.platosMasVendidos().slice(0, 5));
  platosMenosVendidosPreview = computed(() => this.platosMenosVendidos().slice(0, 5));

  lecturaComercial = computed<DashboardLecturaComercial[]>(() => {
    const top = this.platosMasVendidosPreview();
    const bajos = this.platosMenosVendidosPreview();
    if (top.length === 0) return [];

    const lider = top[0];
    const totalTop = top.reduce((total, item) => total + this.extraerImporte(item.detalle), 0);
    const revisar = bajos.filter((_, index) => index < 2).length;

    return [
      {
        titulo: `${lider.nombre} lidera ventas`,
        detalle: `${lider.valor} unidades vendidas en el periodo.`,
        tono: 'success'
      },
      {
        titulo: 'Top 5 con traccion',
        detalle: `Suma ${this.formatCurrency(totalTop)} de facturacion estimada.`,
        tono: 'info'
      },
      {
        titulo: `${revisar} platos para revisar`,
        detalle: 'Priorizar precio, visibilidad o foto antes de pausar.',
        tono: 'warning'
      }
    ];
  });

  recomendacionTopVentas = computed(() => {
    const top = this.platosMasVendidosPreview();
    if (top.length === 0) return 'Sin datos de ventas suficientes.';
    const principales = top.slice(0, 2).map(item => item.nombre).join(' y ');
    return `Usar ${principales} como base para combos o destacados del dia.`;
  });

  accionPlatoBajo(index: number): string {
    if (index === 0) return 'Revisar precio';
    if (index === 1) return 'Mejorar visibilidad';
    return 'Relanzar';
  }

  destinoAccionPlatoBajo(index: number): DashboardDestino {
    return 'carta';
  }

  setPeriodo(periodo: DashboardPeriodo): void {
    this._periodo.set(periodo);
    if (periodo !== 'custom') {
      this._fechaDesde.set('');
      this._fechaHasta.set('');
      this.cargarDatos();
    }
  }

  setFechaDesde(fecha: string): void {
    this._fechaDesde.set(fecha);
    this._periodo.set('custom');
    if (this._fechaHasta()) this.cargarDatos();
  }

  setFechaHasta(fecha: string): void {
    this._fechaHasta.set(fecha);
    this._periodo.set('custom');
    if (this._fechaDesde()) this.cargarDatos();
  }

  cargarDatos(): void {
    // 1. Cargar Vencimientos
    this.api.getVencimientos().subscribe({
      next: (vencimientos) => {
        this._vencimientos.set(vencimientos);
      },
      error: (err) => console.error('Error cargando vencimientos', err)
    });

    // 2. Cargar Rendimiento Comercial
    const fechaHistorica = new Date(2020, 0, 1).toISOString();
    const fechaFutura = new Date();
    fechaFutura.setFullYear(fechaFutura.getFullYear() + 1);
    const fechaFuturaIso = fechaFutura.toISOString();

    this.api.getRendimientoComercial(fechaHistorica, fechaFuturaIso).subscribe({
      next: (res) => {
        this._platosMasVendidos.set(res.masVendidos);
        this._platosMenosVendidos.set(res.menosVendidos);
      },
      error: (err) => console.error('Error cargando rendimiento', err)
    });
  }

  private obtenerRangoFechas(): { desde: Date; hasta: Date } {
    const hoy = new Date();
    let desde = new Date();
    let hasta = new Date();

    switch (this._periodo()) {
      case '1d': desde.setDate(hoy.getDate() - 1); break;
      case '3d': desde.setDate(hoy.getDate() - 3); break;
      case '7d': desde.setDate(hoy.getDate() - 7); break;
      case '30d': desde.setDate(hoy.getDate() - 30); break;
      case '365d': desde.setDate(hoy.getDate() - 365); break;
      case 'custom':
        const parsedDesde = this.parseFecha(this._fechaDesde());
        const parsedHasta = this.parseFecha(this._fechaHasta());
        if (parsedDesde) desde = parsedDesde;
        if (parsedHasta) hasta = parsedHasta;
        break;
    }
    return { desde, hasta };
  }

  private parseFecha(fecha: string): Date | null {
    if (!fecha) return null;
    const partes = fecha.split('/');
    if (partes.length !== 3) return null;
    const [dia, mes, anio] = partes.map(Number);
    if (!dia || !mes || !anio) return null;
    return new Date(anio, mes - 1, dia);
  }

  private extraerImporte(valor: string): number {
    return Number(valor.replace(/[^0-9]/g, '')) || 0;
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS',
      maximumFractionDigits: 0
    }).format(value);
  }

  private factorPeriodo(): number {
    const factorPorPeriodo: Record<DashboardPeriodo, number> = {
      '1d': 0.16,
      '3d': 0.45,
      '7d': 1,
      '30d': 4.2,
      '365d': 48,
      custom: Math.max(0.14, Math.min(52, this.diasPersonalizados() / 7))
    };
    return factorPorPeriodo[this._periodo()];
  }

  private diasPersonalizados(): number {
    const desde = this.parseFecha(this._fechaDesde());
    const hasta = this.parseFecha(this._fechaHasta());
    if (!desde || !hasta) return 7;
    const diff = Math.abs(hasta.getTime() - desde.getTime());
    return Math.max(1, Math.round(diff / 86400000) + 1);
  }

  private ventasPersonalizadas(): DashboardVentaMensual[] {
    const dias = this.diasPersonalizados();
    if (dias <= 7) {
      return MOCK_VENTAS_7D.slice(0, dias).map(item => ({ ...item, ventas: Math.round(item.ventas * 0.92) }));
    }
    if (dias <= 31) {
      return MOCK_VENTAS_30D.map(item => ({ ...item, ventas: Math.round(item.ventas * (dias / 30)) }));
    }
    return MOCK_VENTAS_MENSUALES.map(item => ({ ...item, ventas: Math.round(item.ventas * Math.min(1.2, dias / 365)) }));
  }
}

const MOCK_VENTAS_MENSUALES: DashboardVentaMensual[] = [
  { mes: 'Ene', ventas: 12500000 },
  { mes: 'Feb', ventas: 14200000 },
  { mes: 'Mar', ventas: 15800000 },
  { mes: 'Abr', ventas: 15100000 },
  { mes: 'May', ventas: 16900000 },
  { mes: 'Jun', ventas: 18400000 }
];

const MOCK_VENTAS_30D: DashboardVentaMensual[] = [
  { mes: 'Sem 1', ventas: 4200000 },
  { mes: 'Sem 2', ventas: 4500000 },
  { mes: 'Sem 3', ventas: 4800000 },
  { mes: 'Sem 4', ventas: 4900000 }
];

const MOCK_VENTAS_7D: DashboardVentaMensual[] = [
  { mes: 'Lun', ventas: 450000 },
  { mes: 'Mar', ventas: 480000 },
  { mes: 'Mie', ventas: 520000 },
  { mes: 'Jue', ventas: 680000 },
  { mes: 'Vie', ventas: 950000 },
  { mes: 'Sab', ventas: 1100000 },
  { mes: 'Dom', ventas: 850000 }
];

const MOCK_VENTAS_3D: DashboardVentaMensual[] = [
  { mes: 'Vie', ventas: 950000 },
  { mes: 'Sab', ventas: 1100000 },
  { mes: 'Dom', ventas: 850000 }
];

const MOCK_VENTAS_24H: DashboardVentaMensual[] = [
  { mes: 'Mañana', ventas: 250000 },
  { mes: 'Tarde', ventas: 150000 },
  { mes: 'Noche', ventas: 450000 }
];

const MOCK_FACTURACION_CENTRO: DashboardFacturacionCentro[] = [
  { centro: 'Delivery', total: 8280000, porcentaje: 45, color: 'var(--accent-teal)' },
  { centro: 'Salon', total: 6440000, porcentaje: 35, color: 'var(--accent-orange)' },
  { centro: 'Mostrador', total: 3680000, porcentaje: 20, color: 'var(--surface-border)' }
];

function generarCalendarioMock(): DashboardVentaDia[] {
  const dias = [];
  const start = new Date(2026, 5, 1);
  const formatter = new Intl.DateTimeFormat('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });

  for (let i = 1; i <= 30; i++) {
    const fecha = new Date(start);
    fecha.setDate(i);
    const dayOfWeek = fecha.getDay();

    let base = 400000;
    if (dayOfWeek === 5) base = 850000;
    if (dayOfWeek === 6) base = 1000000;
    if (dayOfWeek === 0) base = 750000;

    const variance = 1 + (Math.random() * 0.4 - 0.2);

    dias.push({
      dia: i.toString(),
      fecha: formatter.format(fecha),
      ventas: Math.round(base * variance)
    });
  }
  return dias;
}

const MOCK_VENTAS_DIAS_MES = generarCalendarioMock();
