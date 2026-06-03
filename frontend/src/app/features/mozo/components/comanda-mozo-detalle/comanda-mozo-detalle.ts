import { Component, computed, input, output, signal } from '@angular/core';
import { KdsContadorTiempo } from '../../../../shared/ui/kds-contador-tiempo/kds-contador-tiempo';
import { Comanda } from '../../../../core/models/comanda/comanda';

@Component({
  selector: 'app-comanda-mozo-detalle',
  imports: [KdsContadorTiempo],
  templateUrl: './comanda-mozo-detalle.html',
  styleUrl: './comanda-mozo-detalle.css',
})
export class ComandaMozoDetalle {
 comanda = input.required<Comanda>();
  cerrar = output<void>();
  entregar = output<{ comandaId: number; articuloComandaIds: number[] }>();

  seleccionados = signal<Set<number>>(new Set());

  readonly itemsNoEntregados = computed(() =>
    this.comanda().items.filter(i => !i.entregado)
  );

  readonly haySeleccion = computed(() => this.seleccionados().size > 0);

  readonly todosSeleccionados = computed(() => {
    const items = this.itemsNoEntregados();
    return items.length > 0 && items.length === this.seleccionados().size;
  });

  isSelected(id: number): boolean {
    return this.seleccionados().has(id);
  }

  toggleItem(id: number): void {
    this.seleccionados.update(set => {
      const nuevo = new Set(set);
      nuevo.has(id) ? nuevo.delete(id) : nuevo.add(id);
      return nuevo;
    });
  }

  toggleTodos(): void {
    if (this.todosSeleccionados()) {
      this.seleccionados.set(new Set());
    } else {
      const ids = this.itemsNoEntregados().map(i => i.id);
      this.seleccionados.set(new Set(ids));
    }
  }

  cancelar(): void {
    this.seleccionados.set(new Set());
  }

  aplicar(): void {
    if (!this.haySeleccion()) return;

    this.entregar.emit({
      comandaId: this.comanda().id,
      articuloComandaIds: Array.from(this.seleccionados()),
    });

    this.seleccionados.set(new Set());
  }

  onCerrar(): void {
    this.cancelar();
    this.cerrar.emit();
  }
} 


