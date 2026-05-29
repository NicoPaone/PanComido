import { Component, ChangeDetectionStrategy, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VencimientosStateService } from '../services/vencimientos.state';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';

@Component({
  selector: 'app-vencimientos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton],
  templateUrl: './aviso-vencimientos.html',
  styleUrls: ['./aviso-vencimientos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VencimientosPage implements OnInit {
  state = inject(VencimientosStateService);

  ngOnInit() {
    this.state.cargarIngredientes();
  }
}
