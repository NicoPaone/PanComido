import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent } from '../components/detalle-receta/detalle-receta';
import { CrearPlatoFormComponent, PlatoFormData } from '../components/crear-plato-form/crear-plato-form';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { CrearPlatoState } from '../services/crear-plato.state';
import { StockMercaderiaState } from '../../stock-mercaderia/services/insumos/stock-mercaderia-state';

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
  private readonly insumoState = inject(StockMercaderiaState);

  // Signals del state
  visible = this.state.visible;
  imagenSelected = this.state.imagenSelected;
  vegano = this.state.vegano;
  vegetariano = this.state.vegetariano;
  celiaco = this.state.celiaco;
  receta = this.state.receta;
  mostrarExito = this.state.mostrarExito;
  mostrarSelectorImagen = this.state.mostrarSelectorImagen;
  costoSugerido = this.state.costoSugerido;
  insumos = this.insumoState.productos;

  // Imágenes disponibles para el plato
  imagenesDisponibles = [
    { url: 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=200&h=150', label: 'Ensalada' },
    { url: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150', label: 'Milanesa' },
    { url: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150', label: 'Papas Fritas' },
    { url: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pasta al pesto' },
    { url: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pizza Muzarella' },
    { url: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&q=80&w=200&h=150', label: 'Hamburguesa' }
  ];

  constructor() {
    // Cargamos insumos del backend
    this.insumoState.cargarMercaderia();

    // Si viene de sugerencia IA, cargamos los ingredientes al form
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

  onAbrirSelectorImagen(): void {
    this.state.abrirSelectorImagen();
  }

  onCerrarSelectorImagen(): void {
    this.state.cerrarSelectorImagen();
  }

  onSeleccionarImagen(url: string): void {
    this.state.seleccionarImagen(url);
  }

  onCerrarExito(): void {
    this.state.setMostrarExito(false);
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  onCancelar(): void {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
