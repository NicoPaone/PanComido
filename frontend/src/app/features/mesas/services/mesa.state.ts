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

  // 2. Estado PÚBLICO (Solo lectura para los componentes)
  mesas = this._mesas.asReadonly();
  loading = this._loading.asReadonly();
  isEditorMode = this._isEditorMode.asReadonly();

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
    // Buscamos el número más alto y le sumamos 1. Si no hay mesas, arrancamos en 1.
    const proximoNumero = mesas.length > 0 ? Math.max(...mesas.map(m => m.numeroMesa)) + 1 : 1;

    const nuevaMesa: Mesa = {
      id: Date.now(), // ID temporal para que el @for de Angular no se rompa. El backend lo ignora al insertar.
      codigoInvitacion: `MESA-TEMP`,
      numeroMesa: proximoNumero,
      cantidadPersonasMax: 4, // Valor por defecto
      estadoMesa: EstadoMesa.Disponible,
      dimensionMesa: { id: 0, forma, imagen: '' },
      posicionXInicio: 15, posicionXfin: 105, // Coordenadas iniciales (múltiplos de 15)
      posicionYinicio: 15, posicionYFin: 105
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
      alert('¡Error! Hay mesas con números duplicados. Corregilo antes de guardar.');
      return;
    }

    // 2. EL ÚNICO PUT (Bulk Save)
    console.log('JSON LISTO PARA MANDAR AL BACKEND CON PUT:', mesas);

    /* Cuando tengas el endpoint, la llamada va acá:
    this.api.actualizarMapaCompleto(mesas).subscribe({
      next: () => {
        this.toggleEditorMode(); // Salimos del editor solo si el back respondió OK
        alert('Mapa guardado con éxito');
      },
      error: () => {
        alert('Error al guardar en la base de datos');
      }
    });
    */

    // Por ahora, como estamos probando el front, simulamos el éxito:
    this.toggleEditorMode();
    alert('Mapa guardado con éxito (Simulado)');
  }
}
