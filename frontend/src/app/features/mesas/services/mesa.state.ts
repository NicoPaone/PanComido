import { App } from './../../../app';
import { Injectable, inject, signal } from '@angular/core';
import { MesaService } from '../../../core/services/mesa.service';
import { Mesa, EstadoMesa, FormaMesa } from '../../../core/models/mesa.model';

@Injectable({ providedIn: 'root' })
export class MesaStateService {
  private api = inject(MesaService);

  // 1. Estado PRIVADO
  private _mesas = signal<Mesa[]>([]);
  private _loading = signal<boolean>(false);
  private _isEditorMode = signal<boolean>(false);
  private _mesasBackup: Mesa[] = [];
  private _notificacion = signal<{mensaje: string, tipo: 'exito' | 'error'} | null>(null);

  // 2. Estado PÚBLICO (Solo lectura para los componentes)
  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  isEditorMode = this._isEditorMode.asReadonly();
  notificacion = this._notificacion.asReadonly();

  // 3. Casos de uso
  cargarMesas(): void {
    this._loading.set(true);
    this.api.getMesas().subscribe({
      next: (data) => {
        this._mesas.set(data);
        this._loading.set(false);
      },
      error: () => this._loading.set(false)
    });
  }

  mostrarNotificacion(mensaje: string, tipo: 'exito' | 'error'): void {
    this._notificacion.set({ mensaje, tipo });

    // A los 3 segundos (3000ms), volvemos el signal a null para que desaparezca
    setTimeout(() => {
      this._notificacion.set(null);
    }, 3000);
  }

  toggleEditorMode(): void {
    const isEditor = this._isEditorMode();
    if (!isEditor) {
      // Al entrar al modo edición, sacamos la "foto" inmutable
      this._mesasBackup = JSON.parse(JSON.stringify(this._mesas()));
    }
    this._isEditorMode.set(!isEditor);
  }

  cancelarEdicion(): void {
    // Restauramos el estado pisándolo con la foto que sacamos antes
    this._mesas.set(JSON.parse(JSON.stringify(this._mesasBackup)));
    this._isEditorMode.set(false);
  }

  // Este método lo va a llamar el mapa cuando sueltes el clic del drag & drop
  moverMesa(id: number, deltaX: number, deltaY: number): void {
    const mesaActual = this._mesas().find(m => m.id === id);
    if (!mesaActual) return;

    const nuevosDatos: Partial<Mesa> = {
      posicionXInicio: mesaActual.posicionXInicio + deltaX,
      posicionXfin: mesaActual.posicionXfin + deltaX,
      posicionYinicio: mesaActual.posicionYinicio + deltaY,
      posicionYFin: mesaActual.posicionYFin + deltaY
    };

    // Optimistic Update: Mutamos el signal para que la mesa quede ahí al instante.
    // Y LISTO. Cero llamadas HTTP acá. Todo se guarda cuando toquen "Guardar Mapa".
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, ...nuevosDatos } : m)
    );
  }

  agregarMesa(forma: FormaMesa): void {
    const mesas = this._mesas();
    const proximoNumero = mesas.length > 0 ? Math.max(...mesas.map(m => m.numeroMesa)) + 1 : 1;

    // 1. Definimos las dimensiones iniciales según la forma elegida
    let ancho = 90;
    let alto = 90;

    switch (forma) {
      case FormaMesa.HorizontalLarga:
        ancho = 150;
        alto = 75;
        break;
      case FormaMesa.HorizontalAlta:
        ancho = 75;
        alto = 150;
        break;
      case FormaMesa.Redonda:
      case FormaMesa.Cuadrada:
      default:
        ancho = 90;
        alto = 90;
        break;
    }

    // 2. Creamos la mesa aplicando la matemática a las coordenadas finales
    const nuevaMesa: Mesa = {
      id: Date.now(),
      codigoInvitacion: `MESA-TEMP`,
      numeroMesa: proximoNumero,
      cantidadPersonasMax: 4,
      estadoMesa: EstadoMesa.Disponible,
      dimensionMesa: { id: 0, forma, imagen: '' },
      posicionXInicio: 15,
      posicionXfin: 15 + ancho, // Calculado dinámicamente
      posicionYinicio: 15,
      posicionYFin: 15 + alto   // Calculado dinámicamente
    };

    this._mesas.update(m => [...m, nuevaMesa]);
  }

  eliminarMesa(id: number): void {
    this._mesas.update(mesas => mesas.filter(m => m.id !== id));
  }

  actualizarNumero(id: number, nuevoNumero: number): void {
    this._mesas.update(mesas =>
      mesas.map(m => m.id === id ? { ...m, numeroMesa: nuevoNumero } : m)
    );
  }

  guardarConfiguracion(): void {
    const mesas = this._mesas();

    // 1. Validar duplicados
    const numeros = mesas.map(m => m.numeroMesa);
    if (new Set(numeros).size !== numeros.length) {
      this.mostrarNotificacion('Hay mesas con números duplicados. Corregilo antes de guardar.', 'error');
      return;
    }

    // 2. Validar colisiones
    for (let i = 0; i < mesas.length; i++) {
      for (let j = i + 1; j < mesas.length; j++) {
        const mesaA = mesas[i];
        const mesaB = mesas[j];

        const seSolapan = !(
          mesaA.posicionXfin <= mesaB.posicionXInicio ||
          mesaA.posicionXInicio >= mesaB.posicionXfin ||
          mesaA.posicionYFin <= mesaB.posicionYinicio ||
          mesaA.posicionYinicio >= mesaB.posicionYFin
        );

        if (seSolapan) {
          this.mostrarNotificacion(`Las mesas ${mesaA.numeroMesa} y ${mesaB.numeroMesa} se están tocando.`, 'error');
          return;
        }
      }
    }

    // 3. Éxito
    console.log('JSON LISTO PARA MANDAR AL BACKEND CON PUT:', mesas);
    this._isEditorMode.set(false);
    this.mostrarNotificacion('Mapa guardado con éxito', 'exito');
  }
}
