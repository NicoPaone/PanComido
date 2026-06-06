import { Component, inject, effect , ChangeDetectionStrategy} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { UpperCasePipe } from '@angular/common';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent } from '../components/detalle-receta/detalle-receta';
import { RecetaIngrediente } from '../../../../core/models/domain/plato';
import { computed } from '@angular/core';
import { CrearPlatoState } from '../services/crear-plato.state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-crear-plato',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, Boton, ToggleComponent, DetalleRecetaComponent, UpperCasePipe],
  templateUrl: './crear-plato.html',
  styleUrl: './crear-plato.css'
})
export class CrearPlatoComponent {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly state = inject(CrearPlatoState);

  platoForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3)]],
    costo: [0, [Validators.required, Validators.min(0.01)]],
    precioVenta: [0, [Validators.required, Validators.min(0.01)]],
    tiempoPreparacion: [15, [Validators.required, Validators.min(1)]],
    tipoPlatoId: ['', [Validators.required]],
    categoriaPlatoId: ['', [Validators.required]],
    descripcion: ['', [Validators.required, Validators.minLength(8)]]
  });

  visible = this.state.visible;
  imagenSelected = this.state.imagenSelected;
  tiposPlato = this.state.tiposPlato;
  categoriasPlato = this.state.categoriasPlato;
  restricciones = this.state.restricciones;
  restriccionesSeleccionadas = this.state.restriccionesSeleccionadas;
  ingredientesDisponibles = this.state.ingredientesDisponibles;
  receta = this.state.receta;
  mostrarExito = this.state.mostrarExito;
  mostrarSelectorImagen = this.state.mostrarSelectorImagen;
  costoSugerido = this.state.costoSugerido;

  mockImages = [
    { url: 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=200&h=150', label: 'Ensalada' },
    { url: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150', label: 'Milanesa' },
    { url: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150', label: 'Papas Fritas' },
    { url: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pasta al pesto' },
    { url: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pizza Muzarella' },
    { url: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&q=80&w=200&h=150', label: 'Hamburguesa' }
  ];

  private readonly formValue = toSignal(this.platoForm.valueChanges, {
    initialValue: this.platoForm.value
  });

  private readonly formStatus = toSignal(this.platoForm.statusChanges, {
    initialValue: this.platoForm.status
  });

  precioEsMenorQueCosto = computed(() => {
    const currentVal = this.formValue();
    const pVenta = currentVal?.precioVenta ?? 0;
    const pCosto = currentVal?.costo ?? 0;
    return pVenta > 0 && pCosto > 0 && pVenta <= pCosto;
  });

  puedeGuardar = computed(() => {
    return this.formStatus() === 'VALID';
  });

  constructor() {
    this.state.cargarDatosFormulario();

    effect(() => {
      const sugerido = this.costoSugerido();
      if (sugerido > 0) {
        this.platoForm.patchValue({ costo: sugerido });
        this.platoForm.get('costo')?.markAsTouched();
      }
    });

    const nav = this.router.getCurrentNavigation();
    const extras = nav?.extras?.state;

    if (extras?.['desde_ia']) {
      this.platoForm.patchValue({
        nombre: extras['nombre'] ?? '',
        descripcion: extras['descripcion'] ?? '',
        tiempoPreparacion: extras['tiempoPreparacion'] ?? 15,
        costo: 0,
        precioVenta: 0,
      });

      const ingredientes: RecetaIngrediente[] = (extras['ingredientes'] ?? []).map((ing: any) => ({
        id: ing.insumoId,
        nombre: ing.nombre,
        cantidad: ing.cantidad,
        unidadMedida: 'GR',
      }));

      this.state.updateReceta(ingredientes);
    }
  }

  toggleRestriccion(id: number) {
    this.state.toggleRestriccion(id);
  }

  isRestriccionSelected(id: number): boolean {
    return this.restriccionesSeleccionadas().includes(id);
  }

  onToggleVisible() {
    this.state.toggleVisible();
  }

  onRecetaCambiada(ingredientes: RecetaIngrediente[]) {
    this.state.updateReceta(ingredientes);
  }

  abrirSelectorImagen() {
    this.state.abrirSelectorImagen();
  }

  cerrarSelectorImagen() {
    this.state.cerrarSelectorImagen();
  }

  seleccionarImagen(url: string) {
    this.state.seleccionarImagen(url);
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      const reader = new FileReader();
      
      reader.onload = (e) => {
        const dataUrl = e.target?.result as string;
        this.seleccionarImagen(dataUrl);
      };
      
      reader.readAsDataURL(file);
    }
  }

guardar() {
  if (this.platoForm.invalid) {
    this.platoForm.markAllAsTouched();
    return;
  }

  const formVal = this.platoForm.value;
  const platoData = {
    nombre: formVal.nombre!,
    costo: formVal.costo!,
    precioVenta: formVal.precioVenta!,
    tiempoPreparacion: formVal.tiempoPreparacion!,
    tipoPlatoId: Number(formVal.tipoPlatoId!),
    categoriaPlatoId: Number(formVal.categoriaPlatoId!),
    descripcion: formVal.descripcion!,
  };

  this.state.guardarPlato(platoData, () => {});
}

  cerrarExito() {
    this.state.setMostrarExito(false);
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  cancelar() {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
