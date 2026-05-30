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

  private _vencimientos = signal<Aviso[]>([
    { id: 'v1', tipo: 'vencimiento', titulo: 'Leche entera', subtitulo: 'Vence: 2026-06-02', info: 'Batch #23 - 5 unidades' },
    { id: 'v2', tipo: 'vencimiento', titulo: 'Aceite', subtitulo: 'Vence: 2026-06-05', info: 'Bidón 5L' }
  ]);

  private _stockBajo = signal<Aviso[]>([
    { id: '4', tipo: 'stock', titulo: 'Harina 0000', subtitulo: 'Stock: 1 KG', info: 'Punto mínimo: 15 KG' },
    { id: '3', tipo: 'stock', titulo: 'Aceite de Girasol', subtitulo: 'Stock: 3 L', info: 'Punto mínimo: 5 L' }
  ]);

  mensaje = this._mensaje.asReadonly();
  searchTerm = this._searchTerm.asReadonly();
  loadingSugerenciasCocina = this._loadingSugerenciasCocina.asReadonly();
  platoAgregadoACarta = this._platoAgregadoACarta.asReadonly();

  vencimientos = computed(() => {
    const term = this._searchTerm().toLowerCase();
    return this._vencimientos().filter(a => 
      a.titulo.toLowerCase().includes(term) || 
      (a.subtitulo && a.subtitulo.toLowerCase().includes(term)) || 
      (a.info && a.info.toLowerCase().includes(term))
    );
  });

  stockBajo = computed(() => {
    const term = this._searchTerm().toLowerCase();
    return this._stockBajo().filter(a => 
      a.titulo.toLowerCase().includes(term) || 
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
        plato.nombre.toLowerCase().includes(term) ||
        (plato.categoria ?? '').toLowerCase().includes(term)
      )
    );
  });

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

  crearPedido(id: string) {
    this._stockBajo.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Pedido creado (mock)');
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
