import { Component, input,output, model } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faSearch, faTimes } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-buscador',
  imports: [FontAwesomeModule],
  templateUrl: '../buscador/buscador.html',
  styleUrl: '../buscador/buscador.css',
})
export class Buscador {

 // Configuración visual
  placeholder = input<string>('Buscar...');
  
  // Estado interno bidireccional
  valor = model<string>('');

  // Emite el valor cada vez que el usuario escribe o limpia
  busquedaCambiada = output<string>();

  // Íconos
  faSearch = faSearch;
  faTimes = faTimes;

  onInput(event: Event) {
    const valorActual = (event.target as HTMLInputElement).value;
    this.valor.set(valorActual);
    this.busquedaCambiada.emit(valorActual);
  }

  limpiar() {
    this.valor.set('');
    this.busquedaCambiada.emit('');
  }
}
