import { Component, inject, input, output , ChangeDetectionStrategy} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Insumo } from '../../../../../core/models/domain/insumo';
import { Bodega } from '../../../../../core/models/domain/bodega';
import { CrearInsumoRequest } from '../../../../../core/models/dtos/requests/crear-insumo.request';

import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { CategoriaInsumo } from '../../../../../core/models/domain/categoria-insumo';
import { UnidadMedida } from '../../../../../core/models/domain/unidad-medida';

export const stockMinimoValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const stockMinimo = control.get('stockMinimo')?.value;
  const stockInicial = control.get('stockInicial')?.value;

  if (stockMinimo !== null && stockInicial !== null && stockInicial < stockMinimo) {
    return { stockInsuficiente: true };
  }
  return null;
};

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-producto-form',
  imports: [ReactiveFormsModule],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoForm {
  private fb = inject(FormBuilder);

  producto = input<Insumo | null>(null);
  bodegas = input<Bodega[]>([]);
  categorias = input<CategoriaInsumo[]>([]);
  unidadesMedida = input<UnidadMedida[]>([]);
  guardar = output<CrearInsumoRequest>();
  cancelado = output<void>();

  form!: FormGroup;
  ngOnInit(){
    this.initForm();

    const prod = this.producto();
    if(prod){
      this.form.patchValue({
        ...prod,stockInicial: prod.stockActual,
         fechaVencimiento: prod.vencimiento ? prod.vencimiento.split('T')[0] : ''
      });
    }

  }
 private initForm(): void {
    this.form = this.fb.group({
      nombre: ['', Validators.required],
      descripcion: [''], 
      precioVentaFinal: [0, Validators.min(0)], 
      stockMinimo: [0, [Validators.required, Validators.min(0)]],
      unidadDeMedidaId: ['', Validators.required],
      stockInicial: [0, [Validators.required, Validators.min(0)]],
      bodegaId: ['', Validators.required],
      categoriaId: ['', Validators.required], 
      fechaVencimiento: ['', Validators.required], 
      tipo: ['Ingrediente'] 
    }, { validators: stockMinimoValidator });

    void 0;
  }
onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      const payload: CrearInsumoRequest = {
        nombre: formValue.nombre,
        descripcion: formValue.descripcion || '',
        precioVentaFinal: Number(formValue.precioVentaFinal) || 0,
        stockMinimo: Number(formValue.stockMinimo),
        categoriaId: Number(formValue.categoriaId),
        unidadDeMedidaId: Number(formValue.unidadDeMedidaId),
        cantidadInicial: Number(formValue.stockInicial),
        bodegaId: Number(formValue.bodegaId),
        fechaVencimiento: formValue.fechaVencimiento 
        // input type="date" deberia devolver YYYY-MM-DD
      };
      void 0;
      this.guardar.emit(payload);
      this.form.reset({
        stockMinimo: 0,
        precioVentaFinal: 0,
        stockInicial: 0,
        tipo: 'Ingrediente'
      });
    } else {
      this.form.markAllAsTouched();
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
