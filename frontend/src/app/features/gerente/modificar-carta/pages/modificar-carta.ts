import { Component, OnInit, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Plato } from '../../../../core/models/domain/plato';
import { ListaPlatosComponent } from '../components/lista-platos/lista-platos';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { ModalEditarPlatoComponent } from '../containers/modal-editar-plato/modal-editar-plato';
import { ModalEliminarPlatoComponent } from '../components/modal-eliminar-plato/modal-eliminar-plato';
import { ModificarCartaStateService } from '../services/modificar-carta.state';

@Component({
  selector: 'app-modificar-carta',
  standalone: true,
  imports: [
    Buscador, 
    Boton, 
    ListaPlatosComponent, 
    Dropdown, 
    PageToolbar,
    ModalEditarPlatoComponent,
    ModalEliminarPlatoComponent
  ],
  templateUrl: './modificar-carta.html',
  styleUrls: ['./modificar-carta.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ModificarCartaComponent implements OnInit {
  private router = inject(Router);
  private state = inject(ModificarCartaStateService);

  layoutMode = signal<'grid' | 'list'>('grid');

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  platos = this.state.platos;
  filteredPlatos = this.state.filteredPlatos;
  platosRecomendados = this.state.platosRecomendados;
  platosComidas = this.state.platosComidas;
  platosBebidas = this.state.platosBebidas;
  explodingPlatoId = this.state.explodingPlatoId;
  platoAEditar = this.state.platoAEditar;
  platoAEliminar = this.state.platoAEliminar;
  selectedCategoria = this.state.selectedCategoria;
  loading = this.state.loading;
  categoriasDisponibles = this.state.categoriasDisponibles;
  tiposBebidaDisponibles = this.state.tiposBebidaDisponibles;
  selectedTipoBebida = this.state.selectedTipoBebida;
  totalBebidasCount = this.state.totalBebidasCount;

  tiposComidaDisponibles = this.state.tiposComidaDisponibles;
  selectedTipoComida = this.state.selectedTipoComida;
  totalComidasCount = this.state.totalComidasCount;

  sortOrder = this.state.sortOrder;

  ngOnInit() {
    this.state.cargarPlatos();
  }

  onTipoBebidaSeleccionado(tipo: string | null) {
    this.state.setTipoBebida(tipo);
  }

  onTipoComidaSeleccionado(tipo: string | null) {
    this.state.setTipoComida(tipo);
  }

  onSortChanged(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.state.setSortOrder(select.value as 'default' | 'ventas-desc' | 'ventas-asc');
  }

  toggleRecomendado(plato: Plato) {
    this.state.toggleRecomendado(plato);
  }

  onSearch(term: string) {
    this.state.setSearchTerm(term);
  }

  toggleVisibility(plato: Plato) {
    this.state.toggleVisibility(plato);
  }

  onEditPlato(plato: Plato) {
    this.state.setPlatoAEditar(plato);
  }

  onDeletePlato(plato: Plato) {
    this.state.setPlatoAEliminar(plato);
  }

  onSavePlato(updatedFields: Partial<Plato>) {
    this.state.savePlato(updatedFields);
  }

  onConfirmDelete() {
    this.state.confirmDelete();
  }

  onCloseModals() {
    this.state.closeModals();
  }

  onCategoriaSeleccionada(categoria: string | null) {
    this.state.setCategoria(categoria);
  }

  irACrearPlato() {
    this.router.navigate(['/staff/gerente/crear-plato']);
  }

  setLayoutMode(mode: 'grid' | 'list') {
    this.layoutMode.set(mode);
  }
}
