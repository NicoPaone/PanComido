import { inject, Injectable, DestroyRef, signal } from '@angular/core';
import { ConfiguracionService } from './configuracion-service';
import { DatosLocal } from '../../../../core/models/domain/datos-local';
import { MetodoPago } from '../../../../core/models/domain/metodo-pago';
import { TurnoLaboral } from '../../../../core/models/domain/turno-laboral';
import { forkJoin } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class ConfiguracionState {

  private api = inject(ConfiguracionService);
  private destroyRef = inject(DestroyRef);

  readonly #datosLocal = signal<DatosLocal | null>(null);
  readonly #metodosPago = signal<MetodoPago[]>([]);
  readonly #turnos = signal<TurnoLaboral[]>([]);
 
  readonly #loading = signal(false);
  readonly #guardando = signal(false);

  readonly #error = signal<string | null>(null);
  readonly #exito = signal<string | null>(null);

  readonly datosLocal = this.#datosLocal.asReadonly();
  readonly metodosPago = this.#metodosPago.asReadonly();
  readonly turnos = this.#turnos.asReadonly();
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
      turnos: this.api.obtenerTurnos()
    })
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: ({datosLocal, metodosPago, turnos}) => {
        this.#datosLocal.set(datosLocal);
        this.#metodosPago.set(metodosPago);
        this.#turnos.set(turnos);
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

  guardarTodo(): void{
    const datosLocal = this.#datosLocal();

    const metodosPago = this.#metodosPago();
    const turnos = this.#turnos();
    if(!datosLocal){
      this.#error.set('No hay datos para guardar. Cargá la página nuevamente');
      return;
    }
    this.#guardando.set(true);
    this.#error.set(null);
    this.#exito.set(null);

    forkJoin({
      datosLocal: this.api.actualizarDatosLocal(datosLocal),
      metodosPago: this.api.actualizarMetodosPago(metodosPago),
      turnos: this.api.actualizarTurnos(turnos)

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
