import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CierreCajaStateService } from '../services/cierre-caja.state';
import { GlassCard } from '../../../../shared/ui/glass-card/glass-card';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ArsCurrencyPipe } from '../../../../shared/pipes/ars-currency.pipe';

@Component({
  selector: 'app-cierre-caja',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    GlassCard,
    Boton,
    ArsCurrencyPipe
  ],
  templateUrl: './cierre-caja.html',
  styleUrls: ['./cierre-caja.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CierreCajaComponent implements OnInit {
  public state = inject(CierreCajaStateService);

  ngOnInit(): void {
    this.state.cargarDatos();
  }

  onEfectivoContadoChange(valor: number): void {
    this.state.setEfectivoContado(valor || 0);
  }

  onObservacionChange(valor: string): void {
    this.state.setObservacion(valor || '');
  }

  onTurnoChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.state.cambiarTurnoId(Number(select.value));
  }

  abrirConfirmacion(): void {
    this.state.abrirConfirmacion();
  }

  cerrarConfirmacion(): void {
    this.state.cerrarConfirmacion();
  }

  abrirDetalleCierre(cierre: any): void {
    this.state.abrirDetalleCierre(cierre);
  }

  cerrarDetalleCierre(): void {
    this.state.cerrarDetalleCierre();
  }

  abrirModalPlatos(tipo: 'mas' | 'menos'): void {
    this.state.abrirModalPlatos(tipo);
  }

  cerrarModalPlatos(): void {
    this.state.cerrarModalPlatos();
  }

  abrirEncuestasDetalle(): void {
    this.state.abrirEncuestasDetalle();
  }

  cerrarEncuestasDetalle(): void {
    this.state.cerrarEncuestasDetalle();
  }

  confirmarCierre(): void {
    this.state.confirmarCierre();
  }

  imprimirReporte(id?: number): void {
    this.state.imprimirReporte(id);
  }

  getEstadoClase(estado: string): string {
    const e = estado?.toLowerCase() || '';
    if (e.includes('cuadra')) return 'estado-cuadrada';
    if (e.includes('sobrante')) return 'estado-sobrante';
    if (e.includes('falta')) return 'estado-faltante';
    return '';
  }
}
