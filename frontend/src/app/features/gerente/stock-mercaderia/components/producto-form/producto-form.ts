import { Component, inject, input, output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Insumo } from '../../../../../core/models/insumos/insumo';

@Component({
  selector: 'app-producto-form',
  imports: [],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoForm {
  private fb = inject(FormBuilder);

  producto = input<Insumo | null>(null);

  guardar = output<any>();
  cancelado = output<void>();

  form!: FormGroup;
  ngOnInit(){
    this.initForm();

    const prod = this.producto();
    if(prod){
      this.form.patchValue(prod);
    }

  }
  private initForm(): void{
    //TODO: agregar validaciones
    this.form = this.fb.group({
      id: [null],
      nombre: [''],
      stockMinimo: [0],
      vencimiento: [''],
      unidadMedida: [''],
      categoriaIngrediente: [''],
      Bodega: [''],
      cantidadInicial: [0],
    });
    
  }
  onSubmit():void{
    if(this.form.valid){
      this.guardar.emit(this.form.value);
    }else{
      this.form.markAllAsTouched();
    }
  }
}
