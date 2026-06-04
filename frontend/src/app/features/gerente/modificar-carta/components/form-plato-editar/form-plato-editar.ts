import { Component, effect, inject, input, output , ChangeDetectionStrategy} from '@angular/core';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from "../../../../../shared/ui/botones/boton/boton";
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-form-plato-editar',
  imports: [Boton, ReactiveFormsModule, CommonModule],
  templateUrl: './form-plato-editar.html',
  styleUrl: './form-plato-editar.css',
})
export class FormPlatoEditar {

  plato = input<Plato | null>(null);
  cancelar = output<void>();
  guardar = output<Partial<Plato>>();
  categorias = input<string[]>(['Principales', 'Entradas', 'Bebidas', 'Postres']);

  private fb = inject(FormBuilder);

  //estructura basica de formulario
  form: FormGroup = this.fb.group({
    nombre: ['', Validators.required],
    precioVenta: [0, [Validators.required, Validators.min(0)]],
    costo: [0, [Validators.required, Validators.min(0)]],
    tiempoPreparacion: [15, [Validators.required, Validators.min(1)]],
    categoria: ['Principales', Validators.required],
    descripcion: ['']
  });
  constructor() {
    // effect reacciona automáticamente cuando el modal se abre y recibe un plato
    effect(() => {
      const platoActual = this.plato();
      if (platoActual) {
        this.form.patchValue({
          nombre: platoActual.nombre,
          precioVenta: platoActual.precioVenta,
          costo: platoActual.costo,
          tiempoPreparacion: platoActual.tiempoPreparacion || 15,
          categoria: platoActual.categoria || 'Principales',
        });
      } else {
        this.form.reset({ categoria: 'Principales', tiempoPreparacion: 15 }); 
      }
    });
  }

  onSubmit(){
    if(this.form.valid){
      this.guardar.emit(this.form.value);
    }
    else{
      this.form.markAllAsTouched();
    }
  }




}
