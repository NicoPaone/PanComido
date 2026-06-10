import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ArsCurrencyPipe } from '../../../../shared/pipes/ars-currency.pipe';
import { CierreCajaStateService, RankingCajaItem, HistorialCierreItem } from '../services/cierre-caja.state';
import { Modal } from '../../../../shared/ui/modal/modal';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-cierre-caja',
  standalone: true,
  imports: [CommonModule, FormsModule, ArsCurrencyPipe, Modal],
  templateUrl: './cierre-caja.html',
  styleUrls: ['./cierre-caja.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CierreCajaPage {
  readonly state = inject(CierreCajaStateService);
  private readonly authService = inject(AuthService);

  readonly modalDetalle = viewChild.required<Modal>('modalDetalle');
  readonly modalConfirmar = viewChild.required<Modal>('modalConfirmar');

  readonly diferenciaAbsoluta = computed(() => Math.abs(this.state.diferenciaEfectivo()));
  readonly cierreSeleccionado = signal<HistorialCierreItem | null>(null);
  readonly descargarPdfAlConfirmar = signal(false);

  readonly salesMeta = computed(() => this.state.turnoSeleccionadoId() === 'dia' ? 600000 : 1000000);
  readonly progressPercentage = computed(() => {
    const total = this.state.totalRecaudado();
    const meta = this.salesMeta();
    return meta > 0 ? Math.round((total / meta) * 100) : 0;
  });
  readonly progressStrokeOffset = computed(() => {
    const pct = Math.min(100, this.progressPercentage());
    const circumference = 2 * Math.PI * 38; // ~238.76
    return circumference - (pct / 100) * circumference;
  });

  porcentajeMedio(monto: number): number {
    const total = this.state.totalRecaudado();
    return total > 0 ? Math.round((monto / total) * 100) : 0;
  }

  anchoRanking(item: RankingCajaItem, items: RankingCajaItem[]): number {
    const max = Math.max(...items.map(ranking => ranking.valor));
    return Math.max(10, Math.round((item.valor / max) * 100));
  }

  trackNombre(_index: number, item: { nombre: string }): string {
    return item.nombre;
  }

  esPasoCompletado(pasoId: string): boolean {
    const pasoActual = this.state.pasoActual();
    const confirmado = this.state.cierreConfirmado();

    if (confirmado) return true;

    if (pasoId === 'turno') {
      return pasoActual === 'conciliacion' || pasoActual === 'confirmacion';
    }
    if (pasoId === 'conciliacion') {
      return pasoActual === 'confirmacion';
    }
    return false;
  }

  logout(): void {
    if (confirm('¿Desea cerrar la sesión actual de la aplicación?')) {
      this.authService.logout();
    }
  }

  verDetalleCierre(cierre: HistorialCierreItem, modal: Modal): void {
    this.cierreSeleccionado.set(cierre);
    modal.abrir();
  }

  abrirConfirmacionCierre(): void {
    this.modalConfirmar().abrir();
  }

  togglePdfOption(): void {
    this.descargarPdfAlConfirmar.update(val => !val);
  }

  ejecutarCierreConConfirmacion(modal: Modal, previewModal: Modal): void {
    const generarPdf = this.descargarPdfAlConfirmar();
    this.state.confirmarCierre();
    modal.cerrar();
    if (generarPdf) {
      previewModal.abrir();
    }
  }

  imprimirReporte(): void {
    window.print();
  }

  generarReportePDF(): void {
    const docHtml = `==================================================
           REPORTE DE CIERRE DE CAJA
                  PanComido
==================================================
Fecha de Turno:     ${this.state.fechaTurno}
Gerente a Cargo:    ${this.state.gerente}
Turno Seleccionado: ${this.state.turnoSeleccionado().nombre}
Horario:            ${this.state.turnoSeleccionado().horario}
Estado de Caja:     ${this.state.cierreConfirmado() ? 'CONFIRMADO' : 'PENDIENTE'}
--------------------------------------------------
VENTAS Y COBROS
--------------------------------------------------
Total Recaudado:    $ ${this.state.totalRecaudado().toLocaleString('es-AR')}
Cobros Registrados: ${this.state.totalOperaciones()} operaciones
--------------------------------------------------
DESGLOSE POR MEDIO DE PAGO
--------------------------------------------------
${this.state.mediosPago().map(medio => `- ${medio.nombre}: $ ${medio.esperado.toLocaleString('es-AR')} (${medio.operaciones} cobros)`).join('\n')}
--------------------------------------------------
CONCILIACIÓN DE EFECTIVO
--------------------------------------------------
Efectivo Esperado:  $ ${this.state.efectivoEsperado().toLocaleString('es-AR')}
Efectivo Contado:   $ ${this.state.efectivoContado().toLocaleString('es-AR')}
Diferencia:         $ ${this.state.diferenciaEfectivo().toLocaleString('es-AR')} (${this.state.estadoCaja()})
--------------------------------------------------
JUSTIFICACIÓN / OBSERVACIÓN
--------------------------------------------------
${this.state.observacion().trim() || 'Sin observaciones registradas.'}
==================================================
Reporte generado el ${new Date().toLocaleString('es-AR')}
==================================================`;

    const blob = new Blob([docHtml], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Reporte_Cierre_${this.state.fechaTurno.replace(/\//g, '-')}_${this.state.turnoSeleccionado().nombre.replace(/\s+/g, '_')}.txt`;
    link.click();
    URL.revokeObjectURL(url);
  }
}
