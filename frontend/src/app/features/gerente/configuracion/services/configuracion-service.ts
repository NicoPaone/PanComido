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
import { FamiliaTipografica } from '../../../../core/models/domain/familia-tipografica';

@Injectable({
  providedIn: 'root',
})
export class ConfiguracionService {
  private api = inject(ApiService);

  private endpoint = "configuracion";

  obtenerDatosLocal(): Observable<DatosLocal> {
    return this.api.get<DatosLocal>(`${this.endpoint}/datos-local`);
  }

  actualizarDatosLocal(datos: DatosLocal, archivo: File | null): Observable<DatosLocal>{
    const formData = datosAFormData(datos, archivo);
    return this.api.put<DatosLocal>(`${this.endpoint}/actualizar-datos`,
      formData
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
  obtenerFamiliasTipograficas(): Observable<FamiliaTipografica[]>{
    return this.api.get<FamiliaTipografica[]>(`${this.endpoint}/familias-tipograficas`);
  }

  //MAPPERS PRIVADOS
  //viven aca porque es la unica capa quenecesitan saber cómo convertir el domain
  // a contrato de backend. DEBATIR SI VA EN infra/mappers (capa externa);

  #aRequestDatosLocal(datos:DatosLocal): ActualizarDatosLocalRequest { 
    return {
      nombre: datos.nombre,
      colorPrincipal: datos.colorPrincipal,
      colorSecundario: datos.colorSecundario,
      textoPrincipal: datos.textoPrincipal,
      textoSecundario: datos.textoSecundario,
      familiaTipograficaId: datos.familiaTipograficaId
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
function datosAFormData(datos: DatosLocal, archivo: File | null) {
  const formData = new FormData();
  formData.append('Nombre', datos.nombre);
  formData.append('ColorPrincipal', datos.colorPrincipal || '#000000');
  if (datos.colorSecundario)
    formData.append('ColorSecundario', datos.colorSecundario);
  if (datos.familiaTipograficaId)
    formData.append('FamiliaTipograficaId', datos.familiaTipograficaId.toString());

 
  if (archivo) {
    formData.append('imagen', archivo);
  }
  return formData;
}

