import { inject, Injectable, DestroyRef, signal } from '@angular/core';
import { ConfiguracionService } from './configuracion-service';
import { DatosLocal } from '../../../../core/models/domain/datos-local';
import { MetodoPago } from '../../../../core/models/domain/metodo-pago';
import { TurnoLaboral } from '../../../../core/models/domain/turno-laboral';
import { forkJoin } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FamiliaTipografica } from '../../../../core/models/domain/familia-tipografica';
import { FilaVirtual } from '../../../../core/models/domain/fila-virtual';
import { PorcentajesGanancia } from '../../../../core/models/domain/porcentajes-ganancia';

@Injectable({
  providedIn: 'root',
})
export class ConfiguracionState {

  private api = inject(ConfiguracionService);
  private destroyRef = inject(DestroyRef);

  readonly #datosLocal = signal<DatosLocal | null>(null);
  readonly #metodosPago = signal<MetodoPago[]>([]);
  readonly #turnos = signal<TurnoLaboral[]>([]);
  readonly #filaVirtual = signal< FilaVirtual | null>(null);
  readonly #porcentajes = signal<PorcentajesGanancia| null>(null);
  readonly #familiasTipograficas = signal<FamiliaTipografica[]>([]);
  readonly #archivoLogoPendiente = signal<File | null> (null);
 
  readonly #loading = signal(false);
  readonly #guardando = signal(false);

  readonly #error = signal<string | null>(null);
  readonly #exito = signal<string | null>(null);

  readonly datosLocal = this.#datosLocal.asReadonly();
  readonly metodosPago = this.#metodosPago.asReadonly();
  readonly turnos = this.#turnos.asReadonly();
  readonly filaVirtual = this.#filaVirtual.asReadonly();
  readonly porcentajes = this.#porcentajes.asReadonly();
  readonly familiasTipograficas = this.#familiasTipograficas.asReadonly();
  readonly loading = this.#loading.asReadonly();
  readonly guardando = this.#guardando.asReadonly();
  readonly error = this.#error.asReadonly();
  readonly exito = this.#exito.asReadonly();

  cargarDatos():void{
    this.#loading.set(true);
    this.#error.set(null);
    this.#exito.set(null);

    forkJoin({
      datosLocal: this.api.obtenerDatosLocal(),
      metodosPago: this.api.obtenerMetodosPago(),
      turnos: this.api.obtenerTurnos(),
      fTipograficas: this.api.obtenerFamiliasTipograficas(),
      filaVirtual: this.api.obtenerFilaVirtual(),
      porcentajes: this.api.obtenerPorcentajes(),
    })
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: ({datosLocal, metodosPago, turnos, fTipograficas, filaVirtual,porcentajes}) => {
        this.#datosLocal.set(datosLocal);
        this.#metodosPago.set(metodosPago);
        this.#turnos.set(turnos);
        this.#filaVirtual.set(filaVirtual);
        this.#porcentajes.set(porcentajes);
        this.#familiasTipograficas.set(fTipograficas);  
        this.#loading.set(false);
        
      },
      error:() => { 
        this.#loading.set(false);
        this.#error.set('No se pueden cargar los datos de configuración');
      }
    })
  }

    actualizarDatosLocal(cambios: Partial<DatosLocal>):void{
      this.#datosLocal.update((actual)=>
        actual ? {...actual,...cambios}: null
      )
    }
    toggleMetodoPago(id:number):void{
      this.#metodosPago.update((lista)=>
        lista.map((m) =>
          m.id=== id ? {...m,habilitado:!m.habilitado}: m
    ))
   }
  actualizarTurno(id:number, 
    cambios: Partial<Pick<
    TurnoLaboral,
    'horarioInicio'|
    'horarioFin'|
    'esNocturno'>>):void{
      this.#turnos.update((lista) =>
      lista.map((t) => (t.id === id ? {...t,...cambios}: t))
    );
  }
  setArchivoLogo(file: File | null): void {
    this.#archivoLogoPendiente.set(file);
    }

    toggleFilaVirtual():void{ 
      this.#filaVirtual.update((actual=> 
        actual? {...actual,habilitada:!actual.habilitada} :null)
      )
    }
    actualizarPorcentajeItem(tipo:'platos' | 'bebidas',id:number,porcentaje:number): void{

      this.#porcentajes.update((actual => {
        if(!actual) return null;
        const items = actual[tipo].map((item) => 
          item.id === id? {...item,porcentaje}:item
        );
        return {...actual,[tipo]:items};
      }))
    }

  guardarTodo(): void{
    const datosLocal = this.#datosLocal();
    const archivo = this.#archivoLogoPendiente();
    const metodosPago = this.#metodosPago();
    const turnos = this.#turnos();
    const filaVirtual =this.#filaVirtual();
    const porcentajes = this.#porcentajes();
    if(!datosLocal || !filaVirtual || !porcentajes){
      this.#error.set('No hay datos para guardar. Cargá la página nuevamente');
      return;
    }
    this.#guardando.set(true);
    this.#error.set(null);
    this.#exito.set(null);
    console.log("Archivo de img: ",archivo)

    forkJoin({
      datosLocal: this.api.actualizarDatosLocal(datosLocal,archivo),
      metodosPago: this.api.actualizarMetodosPago(metodosPago),
      turnos: this.api.actualizarTurnos(turnos),
      filaVirtual: this.api.actualizarFilaVirtual(filaVirtual),
      porcentajes: this.api.actualizarPorcentajes(porcentajes),

    }).pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: (resultado) => {
        if(resultado.datosLocal){
          this.#datosLocal.set(resultado.datosLocal);
        }
        this.#guardando.set(false);
        this.#exito.set('Configuracion guardada correctamente');
      },
      error: () =>{ 
        this.#guardando.set(false);
        this.#error.set("No se pudo guardar la condfiguración. Revisá los datos e intentá nuevamente")
      }
    })
  }

  limpiarFeedback():void{
    this.#error.set(null);
    this.#exito.set(null);
  }



  



}
