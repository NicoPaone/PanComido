import { Component, inject, input, output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Insumo } from '../../../../../core/models/insumos/insumo';
import { Bodega } from '../../../../../core/models/bodega/bodega';
import { CrearInsumoRequest } from '../../../../../core/models/insumos/crear-insumo-request';

import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export const stockMinimoValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const stockMinimo = control.get('stockMinimo')?.value;
  const stockInicial = control.get('stockInicial')?.value;

  if (stockMinimo !== null && stockInicial !== null && stockInicial < stockMinimo) {
    return { stockInsuficiente: true };
  }
  return null;
};

@Component({
  selector: 'app-producto-form',
  imports: [ReactiveFormsModule],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoForm {
  private fb = inject(FormBuilder);

  producto = input<Insumo | null>(null);
  bodegas = input<Bodega[]>([]);
  categorias = input<string[]>([]);

  guardar = output<CrearInsumoRequest>();
  cancelado = output<void>();

  form!: FormGroup;
  ngOnInit(){
    this.initForm();

    const prod = this.producto();
    if(prod){

      this.form.patchValue({
        ...prod,stockInicial: prod.stockActual
      });
    }

  }
  private initForm(): void{
   this.form = this.fb.group({
      nombre: ['', Validators.required],
      stockMinimo: [0, [Validators.required, Validators.min(0)]],
      unidadMedida: ['KG', Validators.required],
      stockInicial: [0, [Validators.required, Validators.min(0)]],
      bodegaId: ['', Validators.required],
      categoria: ['', Validators.required],
      tipo: ['Ingrediente'] 
    }, { validators: stockMinimoValidator });
    
  }
 onSubmit(): void {
    if(this.form.valid) {
      const formValue = this.form.value;
      
      // Armamos el DTO exacto casteando los números (los inputs HTML devuelven strings)
      const payload: CrearInsumoRequest = {
        nombre: formValue.nombre,
        unidadMedida: formValue.unidadMedida,
        stockMinimo: Number(formValue.stockMinimo),
        tipo: formValue.tipo,
        categoria: formValue.categoria,
        stockInicial: Number(formValue.stockInicial),
        bodegaId: Number(formValue.bodegaId)
      };
      
      this.guardar.emit(payload);
      this.form.reset({
        id: null,
        nombre: '',
        stockMinimo: 0,
        unidadMedida: 'KG',
        stockInicial: 0,
        bodegaId: '',
        categoria: '',
        tipo: 'Ingrediente'

      });
    } else {
      this.form.markAllAsTouched(); // Pinta los errores si intentan guardar vacío
    }
  }
  getError(campo:string): string|null{
    const control = this.form.get(campo);

    if (!control || !control.touched) return null;

    if(control.hasError('required')) return 'Este campo es obligatorio';
    if (control.hasError('minlength')) return `Mínimo ${control.getError('minlength').requiredLength} caracteres`;
    if (control.hasError('min')) return `El valor mínimo es ${control.getError('min').min}`;

    if (campo === 'stockInicial' && this.form.hasError('stockInsuficiente')) {
       return 'La cantidad inicial no puede ser menor al stock mínimo';
    }

    return null;
  }
}
