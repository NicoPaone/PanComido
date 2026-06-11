import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ConfiguracionState } from '../../services/configuracion-state';
import { DatosLocalEditables } from '../../../../../core/models/domain/datos-local';
import { TurnoLaboral } from '../../../../../core/models/domain/turno-laboral';
import { DatosLocalForm } from "../../components/datos-local-form/datos-local-form";
import { MetodosPagoList } from "../../components/metodos-pago-list/metodos-pago-list";
import { TurnosLaboralesList } from "../../components/turnos-laborales-list/turnos-laborales-list";

@Component({
  selector: 'app-configuracion-page',
  imports: [DatosLocalForm, MetodosPagoList, TurnosLaboralesList],
  templateUrl: './configuracion-page.html',
  styleUrl: './configuracion-page.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfiguracionPage {
  readonly state = inject(ConfiguracionState);
  readonly datosLocal = this.state.datosLocal;
  readonly metodosPago =this.state.metodosPago;
  readonly turnos = this.state.turnos;
  readonly loading = this.state.loading;
  readonly guardando = this.state.guardando;
  readonly error = this.state.error;
  readonly exito = this.state.exito;

  constructor(){
    this.state.cargarDatos();
  }

  onDatosLocalChange(cambios: Partial<DatosLocalEditables>): void{
    this.state.limpiarFeedback();
    this.state.actualizarDatosLocal(cambios);
  }

  onToggleMetodoPago(id: number): void {
    this.state.toggleMetodoPago(id);
  }

  onTurnoChange(event: {id: number; cambios:Partial<TurnoLaboral>}):void{
    this.state.limpiarFeedback();
    this.state.actualizarTurno(event.id,event.cambios);
  }

}
