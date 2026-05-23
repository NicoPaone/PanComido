import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-buscador',
  standalone: true,
  imports: [],
  templateUrl: './buscador.html',
  styleUrls: ['./buscador.css'],
})
export class Buscador {
  @Input() placeholder: string = 'Buscar...';
  @Output() searchChange = new EventEmitter<string>();

  onInput(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    this.searchChange.emit(inputElement.value);
  }
}
