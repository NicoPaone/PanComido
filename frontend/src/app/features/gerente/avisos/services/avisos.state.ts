import { Injectable, signal, computed } from '@angular/core';
import { Aviso, AvisoTipo } from '../../../../core/models/aviso.model';

@Injectable({ providedIn: 'root' })
export class AvisosStateService {
  private _mensaje = signal<string | null>(null);
  private _searchTerm = signal<string>('');

  private _vencimientos = signal<Aviso[]>([
    { id: 'v1', tipo: 'vencimiento', titulo: 'Leche entera', subtitulo: 'Vence: 2026-06-02', info: 'Batch #23 - 5 unidades' },
    { id: 'v2', tipo: 'vencimiento', titulo: 'Aceite', subtitulo: 'Vence: 2026-06-05', info: 'Bidón 5L' }
  ]);

  private _stockBajo = signal<Aviso[]>([
    { id: '4', tipo: 'stock', titulo: 'Harina 0000', subtitulo: 'Stock: 1 KG', info: 'Punto mínimo: 15 KG' },
    { id: '3', tipo: 'stock', titulo: 'Aceite de Girasol', subtitulo: 'Stock: 3 L', info: 'Punto mínimo: 5 L' }
  ]);

  private _sugerencias = signal<Aviso[]>([
    { id: 'c1', tipo: 'sugerencia', titulo: 'Sugerir: Milanesas', subtitulo: 'Alta demanda', info: 'Se sugiere preparar 30 unidades' }
  ]);

  mensaje = this._mensaje.asReadonly();
  searchTerm = this._searchTerm.asReadonly();

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
    return this._sugerencias().filter(a => 
      a.titulo.toLowerCase().includes(term) || 
      (a.subtitulo && a.subtitulo.toLowerCase().includes(term)) || 
      (a.info && a.info.toLowerCase().includes(term))
    );
  });

  setSearchTerm(term: string) {
    this._searchTerm.set(term);
  }

  marcarRevisado(type: AvisoTipo, id: string) {
    if (type === 'vencimiento') this._vencimientos.update(list => list.filter(a => a.id !== id));
    if (type === 'stock') this._stockBajo.update(list => list.filter(a => a.id !== id));
    if (type === 'sugerencia') this._sugerencias.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Acción realizada');
  }

  crearPedido(id: string) {
    this._stockBajo.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Pedido creado (mock)');
  }

  enviarACocina(id: string) {
    this._sugerencias.update(list => list.filter(a => a.id !== id));
    this.mostrarMensaje('Sugerencia enviada a cocina (mock)');
  }

  marcarTodoRevisado() {
    this._vencimientos.set([]);
    this._stockBajo.set([]);
    this._sugerencias.set([]);
    this.mostrarMensaje('Todos los avisos marcados');
  }

  private mostrarMensaje(msg: string) {
    this._mensaje.set(msg);
    setTimeout(() => this._mensaje.set(null), 2500);
  }
}
