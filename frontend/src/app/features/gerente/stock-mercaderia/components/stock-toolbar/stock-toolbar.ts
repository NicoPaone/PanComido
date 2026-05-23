import { Component, output } from '@angular/core';
import { Buscador } from "../../../../../shared/ui/buscador/buscador";

@Component({
  selector: 'app-stock-toolbar',
  imports: [Buscador],
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
