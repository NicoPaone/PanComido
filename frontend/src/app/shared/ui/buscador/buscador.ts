import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-buscador',
  standalone: true,
  imports: [],
  templateUrl: './buscador.html',
  styleUrls: ['./buscador.css'],
})
export class Buscador {
  @Input() placeholder: string = 'Buscar...';
}
