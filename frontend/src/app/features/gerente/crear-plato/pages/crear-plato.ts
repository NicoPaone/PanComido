import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent } from '../components/detalle-receta/detalle-receta';
import { CrearPlatoFormComponent, PlatoFormData } from '../components/crear-plato-form/crear-plato-form';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { CrearPlatoState } from '../services/crear-plato.state';

interface PlatoIAState {
  desde_ia: boolean;
  nombre?: string;
  descripcion?: string;
  tiempoPreparacion?: number;
  ingredientes?: Array<{ insumoId: number; nombre: string; cantidad: number }>;
}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-crear-plato-page',
  standalone: true,
  imports: [Boton, ToggleComponent, DetalleRecetaComponent, CrearPlatoFormComponent],
  templateUrl: './crear-plato.html',
  styleUrl: './crear-plato.css'
})
export class CrearPlatoPage {
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  private readonly state = inject(CrearPlatoState);

  visible = this.state.visible;
  previsualizarImagen = this.state.previsualizacionImagen;
  tiposPlato = this.state.tiposPlato;
  categoriasPlato = this.state.categoriasPlato;
  restricciones = this.state.restricciones;
  restriccionesSeleccionadas = this.state.restriccionesSeleccionadas;
  ingredientesDisponibles = this.state.ingredientesDisponibles;
  receta = this.state.receta;
  mostrarExito = this.state.mostrarExito;
  costoSugerido = this.state.costoSugerido;
  vegano = this.state.vegano;
  vegetariano = this.state.vegetariano;
  celiaco = this.state.celiaco;
  loading = this.state.loading;
  nombresExistentes = this.state.nombresExistentes;


  constructor() {
    this.state.cargarDatosFormulario();

    const navState = this.location.getState() as PlatoIAState | null;
    if (navState?.desde_ia && navState.ingredientes) {
      const ingredientes: RecetaIngrediente[] = navState.ingredientes.map(ing => ({
        id: ing.insumoId,
        nombre: ing.nombre,
        cantidad: ing.cantidad,
        unidadMedida: 'GR',
      }));

      this.state.updateReceta(ingredientes);
    }
  }

  onGuardar(data: PlatoFormData): void {
    this.state.guardarPlato(data, () => { });
  }

  onToggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco'): void {
    this.state.toggleTag(tag);
  }

  onToggleVisible(): void {
    this.state.toggleVisible();
  }

  onRecetaCambiada(ingredientes: RecetaIngrediente[]): void {
    this.state.updateReceta(ingredientes);
  }

 


  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    const previsualizarUrl = URL.createObjectURL(file);
    this.state.seleccionarImagen(file,previsualizarUrl);
   
  }

  onCerrarExito(): void {
    this.state.setMostrarExito(false);
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  onCancelar(): void {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
