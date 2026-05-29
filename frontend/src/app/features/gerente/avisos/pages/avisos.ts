import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Aviso } from '../../../../core/models/aviso.model';
import { AvisosStateService } from '../services/avisos.state';

@Component({
  selector: 'app-avisos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton, Buscador],
  templateUrl: './avisos.html',
  styleUrls: ['./avisos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AvisosPage {
  state = inject(AvisosStateService);
  router = inject(Router);

  onBuscar(term: string) {
    this.state.setSearchTerm(term);
  }

  abrirAviso(a: Aviso) {
    this.abrirVencimientos(a.id);
  }

  abrirVencimientos(avisoId: string) {
    this.router.navigate(['/staff/gerente/aviso-vencimientos'], { queryParams: { avisoId } });
  }
}
