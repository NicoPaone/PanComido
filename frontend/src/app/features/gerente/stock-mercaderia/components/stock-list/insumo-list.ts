import { ChangeDetectionStrategy, Component, computed, inject, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BreakpointObserver } from '@angular/cdk/layout';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { Insumo } from '../../../../../core/models/domain/insumo';

type StockStatus = 'critical' | 'warning' | 'success';

interface StockRow {
  id: number;
  nombre: string;
  categoria: string;
  stock: string;
  minimo: string;
  vencimiento: string;
  estado: StockStatus;
  estadoLabel: string;
}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-insumo-list',
  imports: [CommonModule, ScrollingModule],
  templateUrl: './insumo-list.html',
  styleUrl: './insumo-list.css',
})
export class InsumoList {
  productos = input.required<Insumo[]>();
  editar = output<number>();

  private readonly breakpointObserver = inject(BreakpointObserver);
  readonly itemSize = toSignal(
    this.breakpointObserver.observe('(max-width: 820px)').pipe(
      map((result) => result.matches ? 152 : 56),
    ),
    { initialValue: 56 },
  );

  private readonly dateFormatter = new Intl.DateTimeFormat('es-AR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  });

  filas = computed<StockRow[]>(() => this.productos().map((item) => {
    const estado = this.estado(item);
    const unidad = item.unidadMedida?.nombre ?? '';

    return {
      id: item.id,
      nombre: item.nombre,
      categoria: item.categoriaIngrediente?.descripcion || 'Sin categoría',
      stock: `${item.stockActual} ${unidad}`.trim(),
      minimo: `${item.stockMinimo} ${unidad}`.trim(),
      vencimiento: this.formatearFecha(item.vencimiento),
      estado,
      estadoLabel: this.estadoLabel(estado),
    };
  }));

  trackById = (_: number, fila: StockRow) => fila.id;

  private estado(item: Insumo): StockStatus {
    if (item.stockActual < item.stockMinimo) return 'critical';
    if (item.stockActual < item.stockMinimo * 2) return 'warning';
    return 'success';
  }

  private estadoLabel(estado: StockStatus): string {
    const labels: Record<StockStatus, string> = {
      critical: 'Crítico',
      warning: 'Bajo',
      success: 'Ok',
    };

    return labels[estado];
  }

  private formatearFecha(value: string): string {
    if (!value) return 'Sin fecha';

    const date = new Date(`${value}T00:00:00`);
    return Number.isNaN(date.getTime()) ? value : this.dateFormatter.format(date);
  }
}
