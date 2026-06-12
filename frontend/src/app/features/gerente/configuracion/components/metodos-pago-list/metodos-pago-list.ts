import { Component, input, output } from '@angular/core';
import { MetodoPago } from '../../../../../core/models/domain/metodo-pago';

@Component({
  selector: 'app-metodos-pago-list',
  imports: [],
  templateUrl: './metodos-pago-list.html',
  styleUrl: './metodos-pago-list.css',
})
export class MetodosPagoList {
  readonly metodosPago = input.required<MetodoPago[]>();
  readonly toggleMetodo = output<number>();

  onToggle(id:number):void{
    this.toggleMetodo.emit(id);
  }

}
