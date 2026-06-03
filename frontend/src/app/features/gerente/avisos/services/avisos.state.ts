import { DestroyRef, Injectable, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Aviso, AvisoTipo } from '../../../../core/models/aviso.model';
import { Plato } from '../../../../core/models/plato';
import { AvisosApiService } from './avisos.api';

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

  mensaje = this._mensaje.asReadonly();
  searchTerm = this._searchTerm.asReadonly();
  loadingSugerenciasCocina = this._loadingSugerenciasCocina.asReadonly();
  platoAgregadoACarta = this._platoAgregadoACarta.asReadonly();

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
          const stockAvisos: Aviso[] = avisos.insumosConStockCritico.map(insumo => ({
            id: insumo.id.toString(),
            tipo: 'stock',
            titulo: insumo.nombre || 'Insumo sin nombre',
            subtitulo: `Stock: ${insumo.stockActual} ${insumo.unidadMedida}`,
            info: `Punto mínimo: ${insumo.stockMinimo} ${insumo.unidadMedida}`,
            payloadStock: insumo
          }));
          this._stockBajo.set(stockAvisos);

          const vencimientosAvisos: Aviso[] = [];
          Object.entries(avisos.insumosConVencimientoProximo).forEach(([insumoIdStr, lotes]) => {
            const insumoId = Number(insumoIdStr);
            const insumoData = insumos.find(i => i.id === insumoId);
            const nombreInsumo = insumoData?.nombre || `Insumo ${insumoIdStr}`;

            lotes.forEach(lote => {
              vencimientosAvisos.push({
                id: lote.id.toString(),
                tipo: 'vencimiento',
                titulo: lote.nombre || `Lote de ${nombreInsumo}`,
                subtitulo: `Vence: ${lote.fechaVencimiento === '0001-01-01' ? 'Sin fecha' : lote.fechaVencimiento}`,
                info: `Cantidad: ${lote.cantidad}`,
                payloadVencimiento: lote
              });
            });
          });
          this._vencimientos.set(vencimientosAvisos);
        },
        error: (err) => console.error('Error al cargar avisos', err)
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
}
