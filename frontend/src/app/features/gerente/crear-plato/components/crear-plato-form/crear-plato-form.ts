import { Component, inject, input, output, computed, ChangeDetectionStrategy, effect } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ItemDesplegableDto } from '../../../services/plato.api';

export interface PlatoFormData {
  nombre: string;
  costo: number;
  precioVenta: number;
  tiempoPreparacion: number;
  tipoPlatoId: number;
  categoriaPlatoId: number;
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
  tiposPlato = input<ItemDesplegableDto[]>([]);
  categoriasPlato = input<ItemDesplegableDto[]>([]);
  vegano = input<boolean>(false);
  vegetariano = input<boolean>(false);
  celiaco = input<boolean>(false);
  loading = input<boolean>(false);

  nombresExistentes = input<string[]>([]);

  // Outputs
  guardar = output<PlatoFormData>();
  toggleTag = output<'vegano' | 'vegetariano' | 'celiaco'>();

  // Form
  platoForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3), (c: any) => this.nombreExisteValidator(c)]],
    costo: [0, [Validators.required, Validators.min(0.01)]],
    precioVenta: [0, [Validators.required, Validators.min(0.01)]],
    tiempoPreparacion: [15, [Validators.required, Validators.min(1)]],
    tipoPlatoId: [null as number | null, [Validators.required]],
    categoriaPlatoId: [null as number | null, [Validators.required]],
    descripcion: ['', [Validators.required, Validators.minLength(8)]]
  });

  private nombreExisteValidator(control: any): { [key: string]: boolean } | null {
    if (!control.value) return null;
    const value = control.value.toLowerCase().trim();
    if (this.nombresExistentes().includes(value)) {
      return { nombreExiste: true };
    }
    return null;
  }

  private readonly sincronizarCostoAutocalculado = effect(() => {
    const costo = this.costoSugerido();
    const costoControl = this.platoForm.controls.costo;

    if (costo > 0 && costoControl.value !== costo) {
      costoControl.setValue(costo);
      costoControl.markAsTouched();
    }
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

  tiempoPreparacionLegible = computed(() => {
    const minutos = Number(this.formValue()?.tiempoPreparacion ?? 0);
    if (!Number.isFinite(minutos) || minutos < 60) return '';

    const horas = Math.floor(minutos / 60);
    const minutosRestantes = minutos % 60;
    return minutosRestantes > 0 ? `${horas} h ${minutosRestantes} min` : `${horas} h`;
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
      tipoPlatoId: formVal.tipoPlatoId!,
      categoriaPlatoId: formVal.categoriaPlatoId!,
      descripcion: formVal.descripcion!,
    });
  }

  onToggleTag(tag: 'vegano' | 'vegetariano' | 'celiaco'): void {
    this.toggleTag.emit(tag);
  }
}
