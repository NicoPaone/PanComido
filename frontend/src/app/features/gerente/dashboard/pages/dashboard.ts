import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, OnDestroy, OnInit, computed, inject, signal, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import flatpickr from 'flatpickr';
import { Instance } from 'flatpickr/dist/types/instance';
import { CustomLocale } from 'flatpickr/dist/types/locale';
import { ArsCurrencyPipe } from '../../../../shared/pipes/ars-currency.pipe';
import { DashboardStateService } from '../services/dashboard.state';
import { DashboardDestino, DashboardPeriodo, DashboardVentaDia } from '../../../../core/models/domain/dashboard';

const SPANISH_LOCALE: CustomLocale = {
  weekdays: {
    shorthand: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
    longhand: ['Domingo', 'Lunes', 'Martes', 'Miercoles', 'Jueves', 'Viernes', 'Sabado']
  },
  months: {
    shorthand: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
    longhand: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre']
  },
  ordinal: () => 'o',
  firstDayOfWeek: 1,
  rangeSeparator: ' a ',
  time_24hr: true
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, ArsCurrencyPipe],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardPage implements OnInit, AfterViewInit, OnDestroy {
  readonly state = inject(DashboardStateService);
  private readonly router = inject(Router);
  private readonly fechaDesdeInput = viewChild<ElementRef<HTMLInputElement>>('fechaDesdeInput');
  private readonly fechaHastaInput = viewChild<ElementRef<HTMLInputElement>>('fechaHastaInput');
  private calendarioDesde: Instance | null = null;
  private calendarioHasta: Instance | null = null;

  readonly diaSeleccionado = signal<DashboardVentaDia | null>(null);

  readonly calendarOffset = computed(() => {
    const dias = this.state.ventasCalendarioMes();
    if (dias.length === 0) return [];

    const primeraFechaStr = dias[0].fecha;
    const partes = primeraFechaStr.split('/');
    if (partes.length !== 3) return [];
    const [dia, mes, anio] = partes.map(Number);
    const fecha = new Date(anio, mes - 1, dia);

    const dayOfWeek = fecha.getDay();
    const offset = dayOfWeek === 0 ? 6 : dayOfWeek - 1;

    return Array.from({ length: offset });
  });

  readonly mayorVenta = computed(() => {
    return Math.max(...this.state.platosMasVendidos().map(item => item.valor));
  });

  readonly mayorVentaMensual = this.state.maxVentasMensuales;

  readonly mejorDiaMes = computed(() => {
    return [...this.state.ventasCalendarioMes()].sort((a, b) => b.ventas - a.ventas)[0] ?? null;
  });

  readonly peorDiaMes = computed(() => {
    return [...this.state.ventasCalendarioMes()].sort((a, b) => a.ventas - b.ventas)[0] ?? null;
  });

  readonly promedioDiaMes = computed(() => {
    const dias = this.state.ventasCalendarioMes();
    if (dias.length === 0) return 0;
    return Math.round(dias.reduce((total, dia) => total + dia.ventas, 0) / dias.length);
  });


  readonly periodos: { label: string; value: DashboardPeriodo }[] = [
    { label: '1 día', value: '1d' },
    { label: '3 días', value: '3d' },
    { label: '1 semana', value: '7d' },
    { label: '1 mes', value: '30d' },
    { label: '1 año', value: '365d' }
  ];

  ngOnInit(): void {
    // Cargar los datos iniciales
    this.state.cargarDatos();
  }

  ngAfterViewInit(): void {
    const baseOptions = {
      locale: SPANISH_LOCALE,
      dateFormat: 'd/m/Y',
      allowInput: true,
      disableMobile: true,
      monthSelectorType: 'dropdown' as const,
      nextArrow: '<span class="material-symbols-outlined">keyboard_arrow_right</span>',
      prevArrow: '<span class="material-symbols-outlined">keyboard_arrow_left</span>',
      onReady: (_dates: Date[], _value: string, instance: Instance) => {
        instance.calendarContainer.classList.add('pancomido-calendar');
      }
    };

    const desde = this.fechaDesdeInput()?.nativeElement;
    const hasta = this.fechaHastaInput()?.nativeElement;

    if (desde) {
      this.calendarioDesde = flatpickr(desde, {
        ...baseOptions,
        onChange: (_dates, value) => this.setFechaDesde(value)
      });
    }

    if (hasta) {
      this.calendarioHasta = flatpickr(hasta, {
        ...baseOptions,
        onChange: (_dates, value) => this.setFechaHasta(value)
      });
    }
  }

  ngOnDestroy(): void {
    this.calendarioDesde?.destroy();
    this.calendarioHasta?.destroy();
  }

  setPeriodo(periodo: DashboardPeriodo): void {
    this.state.setPeriodo(periodo);
    if (periodo !== 'custom') {
      this.calendarioDesde?.clear(false);
      this.calendarioHasta?.clear(false);
    }
  }

  abrirFiltroPersonalizado(): void {
    this.state.setPeriodo('custom');
    const desde = this.fechaDesdeInput()?.nativeElement;
    if (desde) {
      setTimeout(() => {
        desde.focus();
        this.calendarioDesde?.open();
      }, 0);
    } else {
      this.calendarioDesde?.open();
    }
  }

  setFechaDesde(fecha: string): void {
    this.state.setFechaDesde(fecha);
  }

  setFechaHasta(fecha: string): void {
    this.state.setFechaHasta(fecha);
  }

  porcentajeRanking(valor: number): number {
    const max = Math.max(...this.state.platosMasVendidos().map(item => item.valor));
    if (max === 0) return 0;
    return Math.max(8, Math.round((valor / max) * 100));
  }


  porcentajeVentaMensual(valor: number): number {
    return Math.max(10, Math.round((valor / this.mayorVentaMensual()) * 100));
  }

  intensidadVentaDia(valor: number): number {
    const max = this.state.maxVentasCalendarioMes();
    const min = this.peorDiaMes()?.ventas ?? 0;
    if (max <= min) return 0.5;
    return Math.max(0.08, Math.min(1, (valor - min) / (max - min)));
  }

  nivelVentaDia(valor: number): string {
    const intensidad = this.intensidadVentaDia(valor);
    if (intensidad < 0.2) return 'heat-level-1';
    if (intensidad < 0.4) return 'heat-level-2';
    if (intensidad < 0.6) return 'heat-level-3';
    if (intensidad < 0.8) return 'heat-level-4';
    return 'heat-level-5';
  }

  detalleVentaDia(fecha: string, ventas: number): string {
    return `${fecha}: ${new Intl.NumberFormat('es-AR', {
      style: 'currency',
      currency: 'ARS',
      maximumFractionDigits: 0
    }).format(ventas)}`;
  }

  seleccionarDia(item: DashboardVentaDia): void {
    if (this.diaSeleccionado()?.dia === item.dia) {
      this.diaSeleccionado.set(null);
    } else {
      this.diaSeleccionado.set(item);
    }
  }

  cerrarDetalle(): void {
    this.diaSeleccionado.set(null);
  }

  calcularPedidosDia(ventas: number): number {
    return Math.max(1, Math.round(ventas / 4160));
  }

  calcularTicketPromedioDia(ventas: number): number {
    const base = 4160;
    const seed = (ventas % 15) - 7;
    return base + seed * 100;
  }

  esMejorDia(item: DashboardVentaDia): boolean {
    return this.mejorDiaMes()?.dia === item.dia;
  }

  esDiaBajo(item: DashboardVentaDia): boolean {
    return item.ventas < this.promedioDiaMes() * 0.78;
  }

  variacionContraPromedio(item: DashboardVentaDia): number {
    const promedio = this.promedioDiaMes();
    if (!promedio) return 0;
    return Math.round(((item.ventas - promedio) / promedio) * 100);
  }

  irA(destino: DashboardDestino): void {
    if (destino === 'stock') {
      this.router.navigate(['/staff', 'gerente', 'stock-mercaderia'], { fragment: 'lotes' });
      return;
    }

    const routes: Record<DashboardDestino, string[]> = {
      stock: ['/staff', 'gerente', 'stock-mercaderia'], // Handled above
      carta: ['/staff', 'gerente', 'modificar-carta'],
      proveedores: ['/staff', 'gerente', 'ver-proveedores'],
      pedido: ['/staff', 'gerente', 'realizar-pedido-sugerido'],
      vencimientos: ['/staff', 'gerente', 'aviso-vencimientos'] // Will be removed, but kept here just in case type checks complain
    };

    this.router.navigate(routes[destino]);
  }
}
