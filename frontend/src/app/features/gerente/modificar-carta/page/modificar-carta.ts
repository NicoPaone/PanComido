import { Component, OnInit, inject, signal, computed, DestroyRef } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Plato } from '../../../../core/models/plato';
import { ListaPlatosComponent } from '../components/lista-platos/lista-platos';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { PlatoService } from '../../../../core/services/plato.service';
import { ModalEditarPlatoComponent } from '../components/modal-editar-plato/modal-editar-plato';
import { ModalEliminarPlatoComponent } from '../components/modal-eliminar-plato/modal-eliminar-plato';

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
  styleUrls: ['./modificar-carta.css']
})
export class ModificarCartaComponent implements OnInit {
  private router = inject(Router);
  private platoService = inject(PlatoService);
  private destroyRef = inject(DestroyRef);

  searchTerm = signal<string>('');
  platos = signal<Plato[]>([]);

  platoAEditar = signal<Plato | null>(null);
  platoAEliminar = signal<Plato | null>(null);

  filteredPlatos = computed(() => {
    const sorted = [...this.platos()].sort((a, b) => {
      if (a.visible === b.visible) return 0;
      return a.visible ? -1 : 1;
    });

    const lowerTerm = this.searchTerm().toLowerCase().trim();
    if (!lowerTerm) {
      return sorted;
    }
    return sorted.filter(plato => 
      plato.nombre.toLowerCase().includes(lowerTerm)
    );
  });

  explodingPlatoId = signal<number | null>(null);

  ngOnInit() {
    this.platoService.getPlatos()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(platos => {
        this.platos.set(platos);
      });
  }

  onSearch(term: string) {
    this.searchTerm.set(term);
  }

  toggleVisibility(plato: Plato) {
    if (plato.visible) {
      this.explodingPlatoId.set(plato.id);

      setTimeout(() => {
        const targetState = false;
        this.platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));
        this.explodingPlatoId.set(null);

        this.platoService.updatePlato(plato.id, { visible: targetState })
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe({
            next: updated => {
              this.platos.update(platos => platos.map(p => p.id === plato.id ? updated : p));
            },
            error: () => {
              this.platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: true } : p));
            }
          });
      }, 450);
    } else {
      const targetState = true;
      this.platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: targetState } : p));

      this.platoService.updatePlato(plato.id, { visible: targetState })
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: updated => {
            this.platos.update(platos => platos.map(p => p.id === plato.id ? updated : p));
          },
          error: () => {
            this.platos.update(platos => platos.map(p => p.id === plato.id ? { ...p, visible: false } : p));
          }
        });
    }
  }

  onEditPlato(plato: Plato) {
    this.platoAEditar.set(plato);
  }

  onDeletePlato(plato: Plato) {
    this.platoAEliminar.set(plato);
  }

  onSavePlato(updatedFields: Partial<Plato>) {
    const target = this.platoAEditar();
    if (!target) return;

    this.platoService.updatePlato(target.id, updatedFields)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(updated => {
        this.platos.update(platos => platos.map(p => p.id === target.id ? updated : p));
        this.platoAEditar.set(null);
      });
  }

  onConfirmDelete() {
    const target = this.platoAEliminar();
    if (!target) return;

    this.platoService.deletePlato(target.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.platos.update(platos => platos.filter(p => p.id !== target.id));
        this.platoAEliminar.set(null);
      });
  }

  onCloseModals() {
    this.platoAEditar.set(null);
    this.platoAEliminar.set(null);
  }

  onCategoriaSeleccionada(categoria: string) {
    console.log('Categoría seleccionada:', categoria);
  }

  irACrearPlato() {
    this.router.navigate(['/staff/gerente/crear-plato']);
  }
}
