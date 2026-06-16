import { computed, inject, Injectable, signal } from '@angular/core';
import { 
  DashboardPeriodo, DashboardRankingItem, DashboardInsumoVencimiento, 
  DashboardLecturaComercial, DashboardAtencionItem, DashboardAccionItem, 
  DashboardDestino, DashboardVentaMensual, DashboardVentaDia 
} from '../../../../core/models/domain/dashboard';
import { DashboardApiService, DashboardResumenOperativoResponse } from './dashboard.api';

@Injectable({ providedIn: 'root' })
export class DashboardStateService {
  private api = inject(DashboardApiService);

  private _periodo = signal<DashboardPeriodo>('7d');
  private _fechaDesde = signal<string>('');
  private _fechaHasta = signal<string>('');

  private _vencimientos = signal<DashboardInsumoVencimiento[]>([]);
  private _platosMasVendidos = signal<DashboardRankingItem[]>([]);
  private _platosMenosVendidos = signal<DashboardRankingItem[]>([]);
  private _resumen = signal<DashboardResumenOperativoResponse | null>(null);

  periodo = this._periodo.asReadonly();
  fechaDesde = this._fechaDesde.asReadonly();
  fechaHasta = this._fechaHasta.asReadonly();

  insumosPorVencer = this._vencimientos.asReadonly();
  platosMasVendidos = this._platosMasVendidos.asReadonly();
  platosMenosVendidos = this._platosMenosVendidos.asReadonly();
  resumenOperativo = this._resumen.asReadonly();

  variacionVentasEsNegativa = computed(() => this._resumen()?.variacionVentas.startsWith('-') ?? false);
  variacionPedidosEsNegativa = computed(() => this._resumen()?.variacionPedidos.startsWith('-') ?? false);
  variacionTicketEsNegativa = computed(() => this._resumen()?.variacionTicket.startsWith('-') ?? false);

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
    const top = this.platosMasVendidosPreview();

    const items: DashboardAtencionItem[] = [];
    if (top.length > 0) {
      items.push({
        titulo: `${top[0].nombre} es tendencia`,
        detalle: 'Asegura stock de sus ingredientes',
        accion: 'Ver carta',
        destino: 'carta',
        tono: 'info'
      });
    }
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

  acciones = computed<DashboardAccionItem[]>(() => {
    const items: DashboardAccionItem[] = [];
    const criticos = this.insumosPorVencer().filter(i => i.criticidad === 'alta');
    const bajos = this.platosMenosVendidosPreview();

    if (criticos.length > 0) {
      items.push({ titulo: 'Crear pedido sugerido', detalle: 'Reponer insumos criticos', destino: 'pedido', tono: 'danger', impacto: 'Evita quiebres', prioridad: 1 });
      items.push({ titulo: 'Ver vencimientos', detalle: 'Priorizar consumo y descarte', destino: 'stock', tono: 'warning', impacto: 'Reduce merma', prioridad: 2 });
    }

    if (bajos.length > 0) {
      items.push({ titulo: 'Revisar carta', detalle: 'Ajustar baja demanda', destino: 'carta', tono: 'info', impacto: 'Recupera ventas', prioridad: 3 });
    }

    return items;
  });

  esModoCalendario = computed(() => {
    if (this._periodo() === '30d') return true;
    if (this._periodo() === 'custom' && this.diasPersonalizados() > 7 && this.diasPersonalizados() <= 40) return true;
    return false;
  });

  tituloGrafico = computed(() => this.esModoCalendario() ? 'Ventas del mes' : 'Tendencia de ventas');
  subtituloGrafico = computed(() => this.esModoCalendario() ? 'Mapa de calor por dia' : 'Evolucion del periodo');

  ventasMensuales = computed<DashboardVentaMensual[]>(() => {
    const resumen = this._resumen();
    if (!resumen || !resumen.grafico) return [];
    return resumen.grafico.map(g => {
      let etiquetaFormateada = g.etiqueta;
      const partes = g.etiqueta.split('-');
      
      if (partes.length === 3) {
        // Formato yyyy-MM-dd: obtenemos el día de la semana
        const date = new Date(parseInt(partes[0], 10), parseInt(partes[1], 10) - 1, parseInt(partes[2], 10));
        const nombresDias = ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'];
        etiquetaFormateada = nombresDias[date.getDay()];
      } else if (partes.length === 2) {
        // Formato yyyy-MM: obtenemos el nombre completo del mes
        const indexMes = parseInt(partes[1], 10) - 1;
        const nombresMeses = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
        if (indexMes >= 0 && indexMes < 12) {
          etiquetaFormateada = nombresMeses[indexMes];
        }
      }
      
      return {
        mes: etiquetaFormateada,
        ventas: this.extraerImporte(g.total)
      };
    });
  });

  ventasCalendarioMes = computed<DashboardVentaDia[]>(() => {
    const resumen = this._resumen();
    if (!resumen || !resumen.grafico || !this.esModoCalendario()) return [];

    const { desde, hasta } = this.obtenerRangoFechas();
    const ventasPorDia = new Map(resumen.grafico.map(g => [g.etiqueta, this.extraerImporte(g.total)]));
    const resultado: DashboardVentaDia[] = [];

    let actual = new Date(desde);
    while (actual <= hasta) {
      const anio = actual.getFullYear();
      const mes = String(actual.getMonth() + 1).padStart(2, '0');
      const dia = String(actual.getDate()).padStart(2, '0');
      
      const etiqueta = `${anio}-${mes}-${dia}`;
      const fechaFormatted = `${dia}/${mes}/${anio}`;
      
      resultado.push({
        dia: String(actual.getDate()),
        fecha: fechaFormatted,
        ventas: ventasPorDia.get(etiqueta) ?? 0
      });
      
      actual.setDate(actual.getDate() + 1);
    }
    
    return resultado;
  });

  maxVentasCalendarioMes = computed(() => {
    return Math.max(...this.ventasCalendarioMes().map(item => item.ventas));
  });

  maxVentasMensuales = computed(() => {
    return Math.max(...this.ventasMensuales().map(item => item.ventas));
  });

  diasDelPeriodo = computed(() => {
    switch (this._periodo()) {
      case '1d': return 1;
      case '3d': return 3;
      case '7d': return 7;
      case '30d': return 30;
      case '365d': return 365;
      case 'custom': return this.diasPersonalizados();
    }
  });

  promedioDiarioVentas = computed(() => {
    const resumen = this._resumen();
    if (!resumen) return 0;
    const total = this.extraerImporte(resumen.totalVentas);
    const dias = this.diasDelPeriodo();
    return Math.round(total / Math.max(1, dias));
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

    const { desde, hasta } = this.obtenerRangoFechas();
    const desdeIso = desde.toISOString();
    const hastaIso = hasta.toISOString();

    // 2. Cargar Rendimiento Comercial con las fechas reales del filtro
    this.api.getRendimientoComercial(desdeIso, hastaIso).subscribe({
      next: (res) => {
        this._platosMasVendidos.set(res.masVendidos);
        this._platosMenosVendidos.set(res.menosVendidos);
      },
      error: (err) => console.error('Error cargando rendimiento', err)
    });

    // 3. Cargar Resumen Operativo (nuevo endpoint)
    this.api.getResumenOperativo(desdeIso, hastaIso).subscribe({
      next: (resumen) => {
        this._resumen.set(resumen);
      },
      error: (err) => console.error('Error cargando resumen operativo', err)
    });
  }

  private obtenerRangoFechas(): { desde: Date; hasta: Date } {
    const hoy = new Date();
    hoy.setHours(0, 0, 0, 0);

    let desde = new Date(hoy);
    let hasta = new Date(hoy);
    hasta.setHours(23, 59, 59, 999);

    switch (this._periodo()) {
      case '1d': break;
      case '3d': desde.setDate(desde.getDate() - 2); break;
      case '7d': desde.setDate(desde.getDate() - 6); break;
      case '30d': desde.setDate(desde.getDate() - 29); break;
      case '365d': desde.setDate(desde.getDate() - 364); break;
      case 'custom':
        const parsedDesde = this.parseFecha(this._fechaDesde());
        const parsedHasta = this.parseFecha(this._fechaHasta());
        if (parsedDesde) desde = parsedDesde;
        if (parsedHasta) {
          hasta = new Date(parsedHasta);
          hasta.setHours(23, 59, 59, 999);
        }
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

}
