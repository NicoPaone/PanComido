import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VencimientosState } from '../services/vencimientos.state';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';

@Component({
  selector: 'app-vencimientos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton],
  templateUrl: './aviso-vencimientos.html',
  styleUrls: ['./aviso-vencimientos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VencimientosPage implements OnInit {
  state = inject(VencimientosState);

  ngOnInit() {
    this.state.cargarIngredientes();
  }

  nombreUnidad(unidadMedida: UnidadMedida | string | null | undefined): string {
    if (!unidadMedida) return '';
    return typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
  }
}
