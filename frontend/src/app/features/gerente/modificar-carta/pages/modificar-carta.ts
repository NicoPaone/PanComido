import { Component, OnInit, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Plato } from '../../../../core/models/plato';
import { ListaPlatosComponent } from '../components/lista-platos/lista-platos';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { ModalEditarPlatoComponent } from '../components/modal-editar-plato/modal-editar-plato';
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

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  platos = this.state.platos;
  filteredPlatos = this.state.filteredPlatos;
  explodingPlatoId = this.state.explodingPlatoId;
  platoAEditar = this.state.platoAEditar;
  platoAEliminar = this.state.platoAEliminar;

  ngOnInit() {
    this.state.cargarPlatos();
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

  onCategoriaSeleccionada(categoria: string) {
    console.log('Categoría seleccionada:', categoria);
  }

  irACrearPlato() {
    this.router.navigate(['/staff/gerente/crear-plato']);
  }
}
