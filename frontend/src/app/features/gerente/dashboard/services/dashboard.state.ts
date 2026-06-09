import { computed, Injectable, signal } from '@angular/core';

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
}

export type DashboardDestino = 'stock' | 'carta' | 'proveedores' | 'pedido' | 'vencimientos';

const MOCK_KPIS: DashboardKpi[] = [
  { id: 'ventas', label: 'Ventas del periodo', value: '$ 2.845.600', detail: 'Comparado contra el periodo anterior', trend: 12.4, tone: 'revenue' },
  { id: 'pedidos', label: 'Pedidos cerrados', value: '684', detail: 'Promedio diario: 98 pedidos', trend: 8.1, tone: 'orders' },
  { id: 'ticket', label: 'Ticket promedio', value: '$ 4.160', detail: 'Mayor traccion en cena', trend: 5.6, tone: 'success' },
  { id: 'merma', label: 'Insumos por vencer', value: '18', detail: 'Mejoro: 3 menos que el periodo anterior', trend: -3.2, tone: 'warning' }
];

const MOCK_PLATOS_VENDIDOS: DashboardRankingItem[] = [
  { nombre: 'Pizza Muzzarella', valor: 286, detalle: '$ 1.287.000' },
  { nombre: 'Milanesa Napolitana', valor: 241, detalle: '$ 1.205.000' },
  { nombre: 'Papas PanComido', valor: 198, detalle: '$ 594.000' },
  { nombre: 'Hamburguesa Completa', valor: 176, detalle: '$ 968.000' },
  { nombre: 'Ravioles de Verdura', valor: 142, detalle: '$ 781.000' },
  { nombre: 'Ensalada Caesar', valor: 121, detalle: '$ 484.000' },
  { nombre: 'Limonada Jengibre', valor: 108, detalle: '$ 324.000' },
  { nombre: 'Flan Casero', valor: 96, detalle: '$ 268.800' },
  { nombre: 'Empanadas de Carne', valor: 88, detalle: '$ 352.000' },
  { nombre: 'Tiramisu', valor: 73, detalle: '$ 328.500' }
];

const MOCK_PLATOS_MENOS_VENDIDOS: DashboardRankingItem[] = [
  { nombre: 'Sopa de Calabaza', valor: 12, detalle: '$ 42.000' },
  { nombre: 'Risotto de Hongos', valor: 18, detalle: '$ 117.000' },
  { nombre: 'Tortilla Espanola', valor: 21, detalle: '$ 73.500' },
  { nombre: 'Pescado Grillado', valor: 24, detalle: '$ 192.000' },
  { nombre: 'Bruschettas', valor: 29, detalle: '$ 87.000' },
  { nombre: 'Guiso de Lentejas', valor: 33, detalle: '$ 148.500' },
  { nombre: 'Tabla Veggie', valor: 37, detalle: '$ 203.500' },
  { nombre: 'Cafe Irlandes', valor: 42, detalle: '$ 126.000' },
  { nombre: 'Brownie con Helado', valor: 46, detalle: '$ 184.000' },
  { nombre: 'Sandwich Caprese', valor: 51, detalle: '$ 204.000' }
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

const MOCK_INSUMOS_VENCEN: DashboardInsumoVencimiento[] = [
  { nombre: 'Crema de leche', fecha: '2026-06-09', cantidad: '3 Lt', criticidad: 'alta', relativo: 'vence mañana' },
  { nombre: 'Mozzarella', fecha: '2026-06-10', cantidad: '4.75 Kg', criticidad: 'alta', relativo: 'vence en 2 dias' },
  { nombre: 'Tomate perita', fecha: '2026-06-12', cantidad: '6 Kg', criticidad: 'media', relativo: 'vence en 4 dias' },
  { nombre: 'Huevos', fecha: '2026-06-14', cantidad: '36 Un', criticidad: 'media', relativo: 'vence en 6 dias' },
  { nombre: 'Lechuga', fecha: '2026-06-16', cantidad: '2 Kg', criticidad: 'baja', relativo: 'vence en 8 dias' }
];

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
  { titulo: 'Crear pedido sugerido', detalle: 'Reponer insumos criticos', destino: 'pedido', tono: 'danger' },
  { titulo: 'Ver vencimientos', detalle: 'Priorizar consumo y descarte', destino: 'vencimientos', tono: 'warning' },
  { titulo: 'Revisar carta', detalle: 'Ajustar baja demanda', destino: 'carta', tono: 'info' }
];

@Injectable({ providedIn: 'root' })
export class DashboardStateService {
  private _periodo = signal<DashboardPeriodo>('7d');
  private _fechaDesde = signal<string>('');
  private _fechaHasta = signal<string>('');

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
    return this.escalarRanking(MOCK_PLATOS_VENDIDOS, this.factorPeriodo());
  });

  platosMenosVendidos = computed<DashboardRankingItem[]>(() => {
    return this.escalarRanking(MOCK_PLATOS_MENOS_VENDIDOS, Math.max(0.2, this.factorPeriodo() * 0.75));
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
    const horizonte = this.horizonteDias();
    return MOCK_INSUMOS_VENCEN.filter(insumo => this.diasHasta(insumo.fecha) <= horizonte);
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
        detalle: criticos > 0 ? `Vencen dentro de ${this.horizonteDias()} dias` : 'Sin urgencias para este periodo'
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

  maxVentasMensuales = computed(() => {
    return Math.max(...this.ventasMensuales().map(item => item.ventas));
  });

  resumenEjecutivo = computed(() => {
    return 'La facturacion subio 12.4% contra el periodo anterior. Hay 5 insumos con prioridad alta y 3 platos con baja demanda para revisar.';
  });

  platosMasVendidosPreview = computed(() => this.platosMasVendidos().slice(0, 5));

  platosMenosVendidosPreview = computed(() => this.platosMenosVendidos().slice(0, 5));

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
    const base = new Date(2026, 5, 8);
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
}
