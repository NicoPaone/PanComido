import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../../core/services/api-service';
import { Observable } from 'rxjs';
import { DatosLocal } from '../../../../core/models/domain/datos-local';
import { 
   ActualizarDatosLocalRequest,
   ActualizarMetodoPagoRequest, 
   ActualizarTurnoLaboralRequest 
  } from '../../../../core/models/dtos/requests/configuracion.requests';
import { MetodoPago } from '../../../../core/models/domain/metodo-pago';
import { TurnoLaboral } from '../../../../core/models/domain/turno-laboral';

@Injectable({
  providedIn: 'root',
})
export class ConfiguracionService {
  private api = inject(ApiService);

  private endpoint = "configuracion";

  obtenerDatosLocal(): Observable<DatosLocal> {
    return this.api.get<DatosLocal>(`${this.endpoint}/datos-local`);
  }

  actualizarDatosLocal(data: DatosLocal): Observable<DatosLocal>{
    return this.api.put<DatosLocal>(`${this.endpoint}/actualizar-datos`,
      this.#aRequestDatosLocal(data)
    );
  }
  obtenerMetodosPago():Observable<MetodoPago[]>{
    return this.api.get<MetodoPago[]>(`${this.endpoint}/metodos-pago`);
  }
  actualizarMetodosPago(metodos: MetodoPago[]): Observable<void>{
    return this.api.put<void>(`${this.endpoint}/habilitar-metodos-pago`,
      this.#aRequestMetodosPago(metodos));
  }

  obtenerTurnos(): Observable<TurnoLaboral[]>{
    return this.api.get<TurnoLaboral[]>(`${this.endpoint}/turno`);
  }
  actualizarTurnos(data:TurnoLaboral[]):Observable<void>{
    return this.api.put<void>(`${this.endpoint}/actualizar-turno`,
      this.#aRequestTurnos(data)
    );
  }

  //MAPPERS PRIVADOS
  //viven aca porque es la unica capa quenecesitan saber cómo convertir el domain
  // a contrato de backend. DEBATIR SI VA EN infra/mappers (capa externa);

  #aRequestDatosLocal(datos:DatosLocal): ActualizarDatosLocalRequest { 
    return {
      nombre: datos.nombre,
      imagen: datos.imagen,
      colorPrincipal: datos.colorPrincipal,
      colorSecundario: datos.colorSecundario,
      textoPrincipal: datos.textoPrincipal,
      textoSecundario: datos.textoSecundario,
    };
  }

  #aRequestMetodosPago(lista: MetodoPago[]):ActualizarMetodoPagoRequest[]{
    return lista.map((m)=> (
      { 
        id:m.id, 
        habilitado:m.habilitado
      })
    );
  }
  #aRequestTurnos(lista: TurnoLaboral[]):ActualizarTurnoLaboralRequest[]{
    return lista.map((t) => ({
      id: t.id,
      horarioInicio: t.horarioInicio,
      horarioFin: t.horarioFin,
      esNocturno: t.esNocturno,
    }))
  }

}
