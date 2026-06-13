import { computed, Injectable, signal, inject, effect } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';

export type DashboardPeriodo = '1d' | '3d' | '7d' | '30d' | '365d' | 'custom';

export interface DashboardKpi {
  id: string;
  label: string;
  value: string;
  detail: string;
  trend: number;
  tone: 'revenue' | 'orders' | 'warning' | 'success';
}

export interface DashboardRankingItem {
  nombre: string;
  valor: number;
  detalle: string;
}

export interface DashboardVentaMensual {
  mes: string;
  ventas: number;
}

export interface DashboardVentaDia {
  dia: number;
  fecha: string;
  ventas: number;
}

export interface DashboardInsumoVencimiento {
  nombre: string;
  loteNombre?: string;
  fecha: string;
  cantidad: string;
  criticidad: 'alta' | 'media' | 'baja';
  relativo: string;
}

export interface DashboardFacturacionCentro {
  centro: string;
  total: number;
  porcentaje: number;
  color: string;
}

export interface DashboardLecturaCanal {
  titulo: string;
  detalle: string;
}

export interface DashboardLecturaComercial {
  titulo: string;
  detalle: string;
  tono: 'success' | 'warning' | 'info';
}

export interface DashboardAtencionItem {
  titulo: string;
  detalle: string;
  accion: string;
  destino: DashboardDestino;
  tono: 'danger' | 'warning' | 'info';
}

export interface DashboardAccionItem {
  titulo: string;
  detalle: string;
  destino: DashboardDestino;
  tono: 'danger' | 'warning' | 'info';
  impacto: string;
  prioridad: number;
}

export type DashboardDestino = 'stock' | 'carta' | 'proveedores' | 'pedido' | 'vencimientos';

const MOCK_KPIS: DashboardKpi[] = [
  { id: 'ventas', label: 'Ventas del periodo', value: '$ 2.845.600', detail: 'Comparado contra el periodo anterior', trend: 12.4, tone: 'revenue' },
  { id: 'pedidos', label: 'Pedidos cerrados', value: '684', detail: 'Promedio diario: 98 pedidos', trend: 8.1, tone: 'orders' },
  { id: 'ticket', label: 'Ticket promedio', value: '$ 4.160', detail: 'Mayor traccion en cena', trend: 5.6, tone: 'success' },
  { id: 'merma', label: 'Insumos por vencer', value: '18', detail: 'Mejoro: 3 menos que el periodo anterior', trend: -3.2, tone: 'warning' }
];


const MOCK_VENTAS_MENSUALES: DashboardVentaMensual[] = [
  { mes: 'Ene', ventas: 820000 },
  { mes: 'Feb', ventas: 910000 },
  { mes: 'Mar', ventas: 1080000 },
  { mes: 'Abr', ventas: 1260000 },
  { mes: 'May', ventas: 1190000 },
  { mes: 'Jun', ventas: 1420000 },
  { mes: 'Jul', ventas: 1510000 },
  { mes: 'Ago', ventas: 1470000 },
  { mes: 'Sep', ventas: 1630000 },
  { mes: 'Oct', ventas: 1710000 },
  { mes: 'Nov', ventas: 1840000 },
  { mes: 'Dic', ventas: 2140000 }
];

const MOCK_VENTAS_24H: DashboardVentaMensual[] = [
  { mes: '10h', ventas: 42000 },
  { mes: '12h', ventas: 93000 },
  { mes: '14h', ventas: 126000 },
  { mes: '16h', ventas: 78000 },
  { mes: '18h', ventas: 142000 },
  { mes: '20h', ventas: 218000 },
  { mes: '22h', ventas: 184000 }
];

const MOCK_VENTAS_3D: DashboardVentaMensual[] = [
  { mes: 'Lun', ventas: 386000 },
  { mes: 'Mar', ventas: 428000 },
  { mes: 'Mie', ventas: 512000 }
];

const MOCK_VENTAS_7D: DashboardVentaMensual[] = [
  { mes: 'Lun', ventas: 312000 },
  { mes: 'Mar', ventas: 384000 },
  { mes: 'Mie', ventas: 356000 },
  { mes: 'Jue', ventas: 421000 },
  { mes: 'Vie', ventas: 548000 },
  { mes: 'Sab', ventas: 694000 },
  { mes: 'Dom', ventas: 511000 }
];

const MOCK_VENTAS_30D: DashboardVentaMensual[] = [
  { mes: 'Sem 1', ventas: 820000 },
  { mes: 'Sem 2', ventas: 910000 },
  { mes: 'Sem 3', ventas: 1080000 },
  { mes: 'Sem 4', ventas: 1260000 }
];

const MOCK_VENTAS_DIAS_MES: DashboardVentaDia[] = [
  82000, 96000, 124000, 118000, 152000, 184000, 168000,
  76000, 88000, 112000, 146000, 138000, 196000, 218000,
  92000, 104000, 126000, 154000, 174000, 236000, 204000,
  84000, 98000, 135000, 162000, 158000, 224000, 241000,
  108000, 142000
].map((ventas, index) => ({
  dia: index + 1,
  fecha: `${String(index + 1).padStart(2, '0')}/06/2026`,
  ventas
}));


const MOCK_FACTURACION_CENTRO: DashboardFacturacionCentro[] = [
  { centro: 'Salon principal', total: 1480000, porcentaje: 52, color: '#02596c' },
  { centro: 'Delivery', total: 796000, porcentaje: 28, color: '#1691ac' },
  { centro: 'Take away', total: 398000, porcentaje: 14, color: '#f08f1a' },
  { centro: 'Eventos', total: 171600, porcentaje: 6, color: '#f59e0b' }
];

const MOCK_ATENCION: DashboardAtencionItem[] = [
  {
    titulo: '5 insumos criticos',
    detalle: 'Vencen en menos de 72 h',
    accion: 'Ver stock',
    destino: 'stock',
    tono: 'danger'
  },
  {
    titulo: '3 platos en baja',
    detalle: 'Demanda cayo mas de 20%',
    accion: 'Revisar carta',
    destino: 'carta',
    tono: 'warning'
  },
  {
    titulo: 'Delivery en crecimiento',
    detalle: 'Concentra 28% de facturacion',
    accion: 'Ver canal',
    destino: 'proveedores',
    tono: 'info'
  }
];

const MOCK_ACCIONES: DashboardAccionItem[] = [
  { titulo: 'Crear pedido sugerido', detalle: 'Reponer insumos criticos', destino: 'pedido', tono: 'danger', impacto: 'Evita quiebres', prioridad: 1 },
  { titulo: 'Ver vencimientos', detalle: 'Priorizar consumo y descarte', destino: 'vencimientos', tono: 'warning', impacto: 'Reduce merma', prioridad: 2 },
  { titulo: 'Revisar carta', detalle: 'Ajustar baja demanda', destino: 'carta', tono: 'info', impacto: 'Recupera ventas', prioridad: 3 }
];

@Injectable({ providedIn: 'root' })
export class DashboardStateService {
  private api = inject(ApiService);
  private _periodo = signal<DashboardPeriodo>('7d');
  private _fechaDesde = signal<string>('');
  private _fechaHasta = signal<string>('');
  private _insumosBackend = signal<DashboardInsumoVencimiento[]>([]);
  private _platosMasVendidosBackend = signal<DashboardRankingItem[]>([]);
  private _platosMenosVendidosBackend = signal<DashboardRankingItem[]>([]);

  constructor() {
    this.cargarVencimientos();
    
    effect(() => {
      const p = this._periodo();
      const fd = this._fechaDesde();
      const fh = this._fechaHasta();
      if (p === 'custom' && (!fd || !fh)) return;
      this.cargarRendimiento();
    });
  }

  private cargarVencimientos(): void {
    this.api.get<any[]>('gerente/dashboard/vencimientos').subscribe({
      next: (res) => {
        const mapeados: DashboardInsumoVencimiento[] = (res || []).map(item => {
          let fechaNormalizada = item.fecha;
          if (item.fecha && item.fecha.includes('/')) {
            const [dia, mes] = item.fecha.split('/');
            const year = new Date().getFullYear();
            fechaNormalizada = `${year}-${mes}-${dia}`;
          }
          return {
            nombre: item.nombre,
            loteNombre: item.loteNombre,
            fecha: fechaNormalizada,
            cantidad: item.cantidad,
            criticidad: (item.criticidad || 'baja').toLowerCase() as 'alta' | 'media' | 'baja',
            relativo: item.relativo
          };
        });
        this._insumosBackend.set(mapeados);
      },
      error: (err) => console.error('Error cargando vencimientos dashboard', err)
    });
  }

  periodo = this._periodo.asReadonly();
  fechaDesde = this._fechaDesde.asReadonly();
  fechaHasta = this._fechaHasta.asReadonly();

  kpis = computed<DashboardKpi[]>(() => {
    const factor = this.factorPeriodo();
    const facturacion = this.totalFacturacion();
    const pedidos = Math.max(1, Math.round(684 * factor));
    const ticketPromedio = Math.round(facturacion / pedidos);
    const insumos = this.insumosPorVencer().length;

    return MOCK_KPIS.map(kpi => {
      if (kpi.id === 'ventas') {
        return { ...kpi, value: this.formatCurrency(facturacion), trend: this.trendPeriodo(12.4) };
      }
      if (kpi.id === 'pedidos') {
        return { ...kpi, value: pedidos.toString(), detail: `Promedio diario: ${this.promedioDiario(pedidos)} pedidos`, trend: this.trendPeriodo(8.1) };
      }
      if (kpi.id === 'ticket') {
        return { ...kpi, value: this.formatCurrency(ticketPromedio), trend: this.trendPeriodo(5.6) };
      }
      if (kpi.id === 'merma') {
        return { ...kpi, value: insumos.toString(), detail: insumos > 0 ? `${this.insumosCriticos()} con prioridad alta` : 'Sin vencimientos en el periodo' };
      }
      return kpi;
    });
  });

  platosMasVendidos = computed<DashboardRankingItem[]>(() => {
    return this._platosMasVendidosBackend();
  });

  platosMenosVendidos = computed<DashboardRankingItem[]>(() => {
    return this._platosMenosVendidosBackend();
  });

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


  insumosPorVencer = computed<DashboardInsumoVencimiento[]>(() => {
    return this._insumosBackend().filter(insumo => this.diasHasta(insumo.fecha) <= 5);
  });

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

    const maxItems = 3;
    const nombres = criticos.slice(0, maxItems).map(item => item.nombre);
    
    let textoNombres = '';
    if (criticos.length > maxItems) {
      const resto = criticos.length - maxItems;
      textoNombres = nombres.join(', ') + ` y ${resto} más`;
    } else if (nombres.length === 1) {
      textoNombres = nombres[0];
    } else {
      const ultimo = nombres.pop();
      textoNombres = nombres.join(', ') + ' y ' + ultimo;
    }

    return `Priorizar ${textoNombres} en preparaciones del dia para reducir merma y evitar faltantes.`;
  });

  facturacionPorCentro = computed<DashboardFacturacionCentro[]>(() => {
    const factor = this.factorPeriodo();
    return MOCK_FACTURACION_CENTRO.map(centro => ({
      ...centro,
      total: Math.round(centro.total * factor)
    }));
  });

  atencion = computed<DashboardAtencionItem[]>(() => {
    const criticos = this.insumosCriticos();
    const platosEnBaja = this.platosMenosVendidosPreview().length;
    const delivery = this.facturacionPorCentro().find(item => item.centro === 'Delivery')?.porcentaje ?? 0;

    return [
      {
        ...MOCK_ATENCION[0],
        titulo: `${criticos} insumos criticos`,
        detalle: criticos > 0 ? `Vencen dentro de 5 dias` : 'Sin urgencias para este periodo'
      },
      {
        ...MOCK_ATENCION[1],
        titulo: `${platosEnBaja} platos en baja`
      },
      {
        ...MOCK_ATENCION[2],
        detalle: `Concentra ${delivery}% de facturacion`
      }
    ];
  });

  acciones = signal<DashboardAccionItem[]>(MOCK_ACCIONES).asReadonly();

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

  tituloGrafico = computed(() => {
    const periodo = this._periodo();
    if (periodo === '1d') return 'Ventas por hora';
    if (periodo === '3d') return 'Ventas por día';
    if (periodo === '7d') return 'Ventas por día';
    if (periodo === '30d') return 'Distribución diaria de ventas';
    if (periodo === '365d') return 'Ventas por mes';
    if (periodo === 'custom') {
      const dias = this.diasPersonalizados();
      if (dias <= 7) return 'Ventas por día';
      if (dias <= 31) return 'Ventas por semana';
      return 'Ventas por mes';
    }
    return 'Ventas';
  });

  subtituloGrafico = computed(() => {
    const periodo = this._periodo();
    if (periodo === '1d') return 'Distribución horaria del día';
    if (periodo === '3d') return 'Distribución de los últimos 3 días';
    if (periodo === '7d') return 'Distribución semanal';
    if (periodo === '30d') return 'Junio 2026';
    if (periodo === '365d') return 'Variación histórica anual';
    if (periodo === 'custom') {
      const dias = this.diasPersonalizados();
      if (dias <= 7) return 'Distribución diaria personalizada';
      if (dias <= 31) return 'Distribución semanal personalizada';
      return 'Variación mensual personalizada';
    }
    return '';
  });

  maxVentasMensuales = computed(() => {
    return Math.max(...this.ventasMensuales().map(item => item.ventas));
  });

  resumenEjecutivo = computed(() => {
    return 'La facturacion subio 12.4% contra el periodo anterior. Hay 5 insumos con prioridad alta y 3 platos con baja demanda para revisar.';
  });

  platosMasVendidosPreview = computed(() => this.platosMasVendidos().slice(0, 5));

  platosMenosVendidosPreview = computed(() => this.platosMenosVendidos().slice(0, 5));

  lecturaComercial = computed<DashboardLecturaComercial[]>(() => {
    const top = this.platosMasVendidosPreview();
    const bajos = this.platosMenosVendidosPreview();
    
    if (top.length === 0) {
      return [
        {
          titulo: 'Sin datos de ventas',
          detalle: 'No hay platos registrados en este periodo.',
          tono: 'info'
        }
      ];
    }
    
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
    if (top.length === 0) return 'Sin datos suficientes.';
    const principales = top.slice(0, 2).map(item => item.nombre).join(' y ');
    return `Usar ${principales} como base para combos o destacados del dia.`;
  });

  accionPlatoBajo(index: number): string {
    if (index === 0) return 'Revisar precio';
    if (index === 1) return 'Mejorar visibilidad';
    return 'Relanzar';
  }

  destinoAccionPlatoBajo(index: number): DashboardDestino {
    if (index === 0 || index === 1) return 'carta';
    return 'carta';
  }

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

  lecturaCanales = computed<DashboardLecturaCanal[]>(() => {
    const centros = this.facturacionPorCentro();
    const dominante = [...centros].sort((a, b) => b.porcentaje - a.porcentaje)[0];
    const menor = [...centros].sort((a, b) => a.porcentaje - b.porcentaje)[0];
    const fueraSalon = centros
      .filter(centro => !centro.centro.toLowerCase().includes('salon'))
      .reduce((total, centro) => total + centro.porcentaje, 0);

    return [
      {
        titulo: `${dominante.centro} lidera el periodo`,
        detalle: `Concentra ${dominante.porcentaje}% de la facturacion.`
      },
      {
        titulo: 'Canales fuera de salon',
        detalle: `Explican ${fueraSalon}% del total y sostienen el flujo operativo.`
      },
      {
        titulo: `Revisar ${menor.centro}`,
        detalle: `Aporta ${menor.porcentaje}%; evaluar acciones puntuales si hay capacidad disponible.`
      }
    ];
  });

  setPeriodo(periodo: DashboardPeriodo): void {
    this._periodo.set(periodo);
    if (periodo !== 'custom') {
      this._fechaDesde.set('');
      this._fechaHasta.set('');
    }
  }

  setFechaDesde(fecha: string): void {
    this._fechaDesde.set(fecha);
    this._periodo.set('custom');
  }

  setFechaHasta(fecha: string): void {
    this._fechaHasta.set(fecha);
    this._periodo.set('custom');
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

  private horizonteDias(): number {
    const horizontePorPeriodo: Record<DashboardPeriodo, number> = {
      '1d': 1,
      '3d': 3,
      '7d': 7,
      '30d': 30,
      '365d': 365,
      custom: Math.max(1, this.diasPersonalizados())
    };

    return horizontePorPeriodo[this._periodo()];
  }

  private diasPersonalizados(): number {
    const desde = this.parseFecha(this._fechaDesde());
    const hasta = this.parseFecha(this._fechaHasta());
    if (!desde || !hasta) return 7;

    const diff = Math.abs(hasta.getTime() - desde.getTime());
    return Math.max(1, Math.round(diff / 86400000) + 1);
  }

  private parseFecha(fecha: string): Date | null {
    const partes = fecha.split('/');
    if (partes.length !== 3) return null;

    const [dia, mes, anio] = partes.map(Number);
    if (!dia || !mes || !anio) return null;

    return new Date(anio, mes - 1, dia);
  }

  private diasHasta(fecha: string): number {
    const base = new Date();
    const target = new Date(`${fecha}T00:00:00`);
    return Math.max(1, Math.round((target.getTime() - base.getTime()) / 86400000));
  }

  private escalarRanking(items: DashboardRankingItem[], factor: number): DashboardRankingItem[] {
    return items.map(item => {
      const valor = Math.max(1, Math.round(item.valor * factor));
      const importe = this.extraerImporte(item.detalle);
      return {
        ...item,
        valor,
        detalle: this.formatCurrency(Math.round(importe * factor))
      };
    });
  }

  private extraerImporte(valor: string): number {
    return Number(valor.replace(/[^0-9]/g, '')) || 0;
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

  private promedioDiario(pedidos: number): number {
    return Math.max(1, Math.round(pedidos / this.horizonteDias()));
  }

  private insumosCriticos(): number {
    return this.insumosPorVencer().filter(insumo => insumo.criticidad === 'alta').length;
  }

  private trendPeriodo(base: number): number {
    const ajuste = this._periodo() === '1d' ? -4.2 : this._periodo() === '365d' ? 18.6 : base;
    return Number(ajuste.toFixed(1));
  }

  private formatCurrency(value: number): string {
    return new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS',
      maximumFractionDigits: 0
    }).format(value);
  }

  private cargarRendimiento(): void {
    const { desde, hasta } = this.getFechasPeriodo();
    const isoDesde = desde.toISOString().split('T')[0];
    const isoHasta = hasta.toISOString().split('T')[0];

    this.api.get<any>(`gerente/dashboard/rendimiento?desde=${isoDesde}&hasta=${isoHasta}`).subscribe({
      next: (res) => {
        if (!res) return;
        const masVendidosData = res.masVendidos || res.MasVendidos || [];
        const menosVendidosData = res.menosVendidos || res.MenosVendidos || [];

        const masVendidos = masVendidosData.map((item: any) => ({
          nombre: item.nombre || item.Nombre,
          valor: Number((item.unidades || item.Unidades)?.replace(/[^0-9]/g, '') || 0),
          detalle: item.facturacion || item.Facturacion
        })).filter((item: any) => item.valor > 0);
        
        const menosVendidos = menosVendidosData.map((item: any) => ({
          nombre: item.nombre || item.Nombre,
          valor: Number((item.unidades || item.Unidades)?.replace(/[^0-9]/g, '') || 0),
          detalle: item.facturacion || item.Facturacion
        })).filter((item: any) => item.valor > 0);
        
        this._platosMasVendidosBackend.set(masVendidos);
        this._platosMenosVendidosBackend.set(menosVendidos);
      },
      error: (err) => console.error('Error cargando rendimiento dashboard', err)
    });
  }

  private getFechasPeriodo(): { desde: Date, hasta: Date } {
    const periodo = this._periodo();
    const hasta = new Date();
    const desde = new Date();
    
    if (periodo === '1d') {
      desde.setDate(hasta.getDate() - 1);
    } else if (periodo === '3d') {
      desde.setDate(hasta.getDate() - 3);
    } else if (periodo === '7d') {
      desde.setDate(hasta.getDate() - 7);
    } else if (periodo === '30d') {
      desde.setDate(hasta.getDate() - 30);
    } else if (periodo === '365d') {
      desde.setDate(hasta.getDate() - 365);
    } else if (periodo === 'custom') {
      const fd = this.parseFecha(this._fechaDesde());
      const fh = this.parseFecha(this._fechaHasta());
      if (fd && fh) {
        return { desde: fd, hasta: fh };
      } else {
        desde.setDate(hasta.getDate() - 7);
      }
    }
    return { desde, hasta };
  }
}
