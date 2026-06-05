import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Aviso, AvisoTipo } from '../../../../core/models/domain/aviso';
import { Plato } from '../../../../core/models/domain/plato';
import { AvisosApiService } from './avisos.api';
import { SugerenciaIA } from '../../../../core/models/dtos/responses/sugerencia-ia.response';
import { PlatoSugeridoIA } from '../../../../core/models/dtos/responses/sugerencia-ia.response';
import { mapAvisosResponseToDomain } from '../../../../infra/http/mappers/aviso.mapper';
@Injectable({ providedIn: 'root' })
export class AvisosStateService {
  private api = inject(AvisosApiService);
  private destroyRef = inject(DestroyRef);

  private _mensaje = signal<string | null>(null);
  private _searchTerm = signal<string>('');
  private _loadingSugerenciasCocina = signal<boolean>(false);
  private _sugerenciasCocina = signal<Plato[]>([]);
  private _sugerenciasIgnoradas = signal<number[]>([]);
  private _platoAgregadoACarta = signal<Plato | null>(null);

  private _vencimientos = signal<Aviso[]>([]);
  private _stockBajo = signal<Aviso[]>([]);
  private _sugerenciasIA = signal<SugerenciaIA | null>(null);
  private _loadingIA = signal<boolean>(false);
  private _errorIA = signal<string | null>(null);
  private _creandoPlato = signal<number | null>(null);
  private _platoIACreado = signal<string | null>(null);

  mensaje = this._mensaje.asReadonly();
  searchTerm = this._searchTerm.asReadonly();
  loadingSugerenciasCocina = this._loadingSugerenciasCocina.asReadonly();
  platoAgregadoACarta = this._platoAgregadoACarta.asReadonly();
  sugerenciasIA = this._sugerenciasIA.asReadonly();
  loadingIA = this._loadingIA.asReadonly();
  errorIA = this._errorIA.asReadonly();
  creandoPlato = this._creandoPlato.asReadonly();
  platoIACreado = this._platoIACreado.asReadonly();

  vencimientos = computed(() => {
    const term = this._searchTerm().toLowerCase();
    return this._vencimientos().filter(a =>
      (a.titulo || '').toLowerCase().includes(term) ||
      (a.subtitulo && a.subtitulo.toLowerCase().includes(term)) ||
      (a.info && a.info.toLowerCase().includes(term))
    );
  });

  stockBajo = computed(() => {
    const term = this._searchTerm().toLowerCase();
    return this._stockBajo().filter(a =>
      (a.titulo || '').toLowerCase().includes(term) ||
      (a.subtitulo && a.subtitulo.toLowerCase().includes(term)) ||
      (a.info && a.info.toLowerCase().includes(term))
    );
  });

  sugerencias = computed(() => {
    const term = this._searchTerm().toLowerCase();
    const ignoradas = this._sugerenciasIgnoradas();

    return this._sugerenciasCocina().filter(plato =>
      !plato.visible &&
      !ignoradas.includes(plato.id) &&
      (
        (plato.nombre || '').toLowerCase().includes(term) ||
        (plato.categoria ?? '').toLowerCase().includes(term)
      )
    );
  });

  cargarAvisos(): void {
    import('rxjs').then(({ forkJoin }) => {
      forkJoin({
        avisos: this.api.getAvisos(),
        insumos: this.api.getInsumos()
      })
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: ({ avisos, insumos }) => {
            const insumosMap = new Map(insumos.map(i => [i.id, i]));
            const { vencimientos, stockBajo } = mapAvisosResponseToDomain(avisos, insumosMap);
            
            this._stockBajo.set(stockBajo);
            this._vencimientos.set(vencimientos);
          },
          error: (err) => void 0
        });
    });
  }

  cargarSugerenciasCocina(): void {
    this._loadingSugerenciasCocina.set(true);
    this.api.getPlatos()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: platos => {
          this._sugerenciasCocina.set(platos);
          this._loadingSugerenciasCocina.set(false);
        },
        error: () => this._loadingSugerenciasCocina.set(false)
      });
  }

  setSearchTerm(term: string) {
    this._searchTerm.set(term);
  }

  marcarRevisado(type: AvisoTipo, id: string) {
    if (type === 'vencimiento') this._vencimientos.update(list => list.filter(a => a.id !== id));
    if (type === 'stock') this._stockBajo.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Acción realizada');
  }

  avisarCocina(id: string) {
    this._vencimientos.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Cocina notificada');
  }

  crearPedido(id: string) {
    this._stockBajo.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Pedido creado');
  }

  agregarSugerenciaACarta(plato: Plato): void {
    this._sugerenciasCocina.update(platos =>
      platos.map(item => item.id === plato.id ? { ...item, visible: true } : item)
    );

    this.api.updatePlato(plato.id, { visible: true })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: updated => {
          this._sugerenciasCocina.update(platos =>
            platos.map(item => item.id === plato.id ? updated : item)
          );
          this._platoAgregadoACarta.set(updated);
        },
        error: () => {
          this._sugerenciasCocina.update(platos =>
            platos.map(item => item.id === plato.id ? { ...item, visible: false } : item)
          );
        }
      });
  }

  ignorarSugerencia(plato: Plato): void {
    this._sugerenciasIgnoradas.update(ids => [...ids, plato.id]);
    this.mostrarMensaje('Sugerencia descartada');
  }

  cerrarConfirmacionCarta(): void {
    this._platoAgregadoACarta.set(null);
  }

  marcarTodoRevisado() {
    this._vencimientos.set([]);
    this._stockBajo.set([]);
    this._sugerenciasIgnoradas.set(this._sugerenciasCocina().map(plato => plato.id));
    this.mostrarMensaje('Todos los avisos marcados');
  }

  private mostrarMensaje(msg: string) {
    this._mensaje.set(msg);
    setTimeout(() => this._mensaje.set(null), 2500);
  }

  generarSugerenciasIA(): void {
    this._loadingIA.set(true);
    this._errorIA.set(null);
    this._sugerenciasIA.set(null);

    this.api.generarSugerenciasIA()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (resultado) => {
          this._sugerenciasIA.set(resultado);
          this._loadingIA.set(false);
        },
        error: () => {
          this._errorIA.set('No se pudo generar la sugerencia. Intentá de nuevo.');
          this._loadingIA.set(false);
        }
      });
  }

  crearPlatoDesdeIA(plato: PlatoSugeridoIA): void {
  this._creandoPlato.set(plato.id);

  const request = {
    nombre: plato.nombre,
    descripcion: plato.descripcion,
    precioVentaFinal: 0,
    tiempoPreparacionBase: plato.tiempoPreparacion,
    tipoPlatoId: 2,
    categoriaPlatoId: 2,
    urlImagen: '',
    restriccionesIds: [],
    ingredientes: plato.ingredientesSugeridosIA.map(ing => ({
      insumoId: ing.insumoId,
      cantidad: ing.cantidad,
      opcional: false
    }))
  };

  this.api.crearPlatoDesdeIA(request)
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: () => {
        this._creandoPlato.set(null);
        this._platoIACreado.set(plato.nombre);
        setTimeout(() => this._platoIACreado.set(null), 3000);
      },
      error: () => {
        this._creandoPlato.set(null);
        this.mostrarMensaje('No se pudo crear el plato. Intentá de nuevo.');
      }
    });
}
}
