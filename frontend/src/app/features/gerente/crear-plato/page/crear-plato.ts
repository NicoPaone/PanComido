import { Component, inject, signal, computed, effect, DestroyRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../shared/ui/toggle/toggle';
import { DetalleRecetaComponent } from '../components/detalle-receta/detalle-receta';
import { RecetaIngrediente } from '../../../../core/models/plato';
import { PlatoService, calcularCostoReceta } from '../../../../core/services/plato.service';

@Component({
  selector: 'app-crear-plato',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, Boton, ToggleComponent, DetalleRecetaComponent],
  templateUrl: './crear-plato.html',
  styleUrl: './crear-plato.css'
})
export class CrearPlatoComponent {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly platoService = inject(PlatoService);
  private readonly destroyRef = inject(DestroyRef);

  platoForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3)]],
    costo: [0, [Validators.required, Validators.min(0.01)]],
    precioVenta: [0, [Validators.required, Validators.min(0.01)]],
    tiempoPreparacion: [15, [Validators.required, Validators.min(1)]],
    tipoPlato: ['', [Validators.required]],
    descripcion: ['', [Validators.required, Validators.minLength(8)]]
  });

  visible = signal<boolean>(true);
  imagenSelected = signal<string>('https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=200&h=150');
  
  vegano = signal<boolean>(false);
  vegetariano = signal<boolean>(false);
  celiaco = signal<boolean>(false);

  receta = signal<RecetaIngrediente[]>([]);
  mostrarExito = signal<boolean>(false);
  mostrarSelectorImagen = signal<boolean>(false);

  mockImages = [
    { url: 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&q=80&w=200&h=150', label: 'Ensalada' },
    { url: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150', label: 'Milanesa' },
    { url: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150', label: 'Papas Fritas' },
    { url: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pasta al pesto' },
    { url: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150', label: 'Pizza Muzarella' },
    { url: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?auto=format&fit=crop&q=80&w=200&h=150', label: 'Hamburguesa' }
  ];

  // Convertimos los observables de valor y estado del formulario a Signals
  private readonly formValue = toSignal(this.platoForm.valueChanges, {
    initialValue: this.platoForm.value
  });

  private readonly formStatus = toSignal(this.platoForm.statusChanges, {
    initialValue: this.platoForm.status
  });

  costoSugerido = computed(() => {
    return calcularCostoReceta(this.receta());
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
    effect(() => {
      const sugerido = this.costoSugerido();
      if (sugerido > 0) {
        this.platoForm.patchValue({ costo: sugerido });
        this.platoForm.get('costo')?.markAsTouched();
      }
    });
  }

  toggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco') {
    if (tag === 'vegano') {
      this.vegano.update(v => !v);
    } else if (tag === 'vegetariano') {
      this.vegetariano.update(v => !v);
    } else if (tag === 'celiaco') {
      this.celiaco.update(v => !v);
    }
  }

  onToggleVisible() {
    this.visible.update(v => !v);
  }

  onRecetaCambiada(ingredientes: RecetaIngrediente[]) {
    this.receta.set(ingredientes);
  }

  abrirSelectorImagen() {
    this.mostrarSelectorImagen.set(true);
  }

  cerrarSelectorImagen() {
    this.mostrarSelectorImagen.set(false);
  }

  seleccionarImagen(url: string) {
    this.imagenSelected.set(url);
    this.mostrarSelectorImagen.set(false);
  }

  guardar() {
    if (this.platoForm.invalid) {
      return;
    }

    const formVal = this.platoForm.value;
    const nuevoPlato = {
      nombre: formVal.nombre!,
      costo: formVal.costo!,
      precioVenta: formVal.precioVenta!,
      visible: this.visible(),
      imagen: this.imagenSelected(),
      receta: this.receta()
    };

    // NOTE: El endpoint del back para registrar un nuevo plato debe conectarse aquí
    this.platoService.crearPlato(nuevoPlato)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.mostrarExito.set(true);
        },
        error: () => {
          // NOTE: El manejo de errores de comunicación debe integrarse aquí
        }
      });
  }

  cerrarExito() {
    this.mostrarExito.set(false);
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }

  cancelar() {
    this.router.navigate(['/staff/gerente/modificar-carta']);
  }
}
