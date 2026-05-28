import { Component, inject, input, output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Insumo } from '../../../../../core/models/producto-stock';

@Component({
  selector: 'app-producto-form',
  imports: [],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoForm {
  private fb = inject(FormBuilder);

  producto = input<Insumo | null>(null);

  salvado = output<any>();
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
      stock: [0],
      fechaVencimiento: [''],
      unidadMedida: [''],
      categoriaIngrediente: [''],
      stockMinimo: [0],
    });
    
  }
  onSubmit():void{
    if(this.form.valid){
      this.salvado.emit(this.form.value);
    }else{
      this.form.markAllAsTouched();
    }
  }
}
