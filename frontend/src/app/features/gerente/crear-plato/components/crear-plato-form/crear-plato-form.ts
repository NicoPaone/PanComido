import { Component, inject, input, output, computed, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';

export interface PlatoFormData {
  nombre: string;
  costo: number;
  precioVenta: number;
  tiempoPreparacion: number;
  tipoPlato: string;
  descripcion: string;
}

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-crear-plato-form',
  standalone: true,
  imports: [ReactiveFormsModule, Boton],
  templateUrl: './crear-plato-form.html',
  styleUrl: './crear-plato-form.css'
})
export class CrearPlatoFormComponent {
  private readonly fb = inject(FormBuilder);

  // Inputs
  costoSugerido = input<number>(0);
  vegano = input<boolean>(false);
  vegetariano = input<boolean>(false);
  celiaco = input<boolean>(false);

  // Outputs
  guardar = output<PlatoFormData>();
  toggleTag = output<'vegano' | 'vegetariano' | 'celiaco'>();

  // Form
  platoForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3)]],
    costo: [0, [Validators.required, Validators.min(0.01)]],
    precioVenta: [0, [Validators.required, Validators.min(0.01)]],
    tiempoPreparacion: [15, [Validators.required, Validators.min(1)]],
    tipoPlato: ['', [Validators.required]],
    descripcion: ['', [Validators.required, Validators.minLength(8)]]
  });

  // Signals derivados del form
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

  onGuardar(): void {
    if (this.platoForm.invalid) {
      this.platoForm.markAllAsTouched();
      return;
    }

    const formVal = this.platoForm.value;
    this.guardar.emit({
      nombre: formVal.nombre!,
      costo: formVal.costo!,
      precioVenta: formVal.precioVenta!,
      tiempoPreparacion: formVal.tiempoPreparacion!,
      tipoPlato: formVal.tipoPlato!,
      descripcion: formVal.descripcion!,
    });
  }

  onToggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco'): void {
    this.toggleTag.emit(tag);
  }
}
