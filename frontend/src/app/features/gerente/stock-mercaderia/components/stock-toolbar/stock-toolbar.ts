import { Component, output } from '@angular/core';

@Component({
  selector: 'app-stock-toolbar',
  imports: [],
  templateUrl: './stock-toolbar.html',
  styleUrl: './stock-toolbar.css',
})
export class StockToolbar {
  busquedaCambiada = output<string>();
  categoriaCambiada = output<string>();
  
  onBuscar(termino: string){
    this.busquedaCambiada.emit(termino);
  }
}
