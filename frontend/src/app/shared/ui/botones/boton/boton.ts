import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-boton',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './boton.html',
  styleUrls: ['./boton.css'],
})
export class Boton {
  @Input() variant: 'teal' | 'orange' | 'edit' | 'add' | 'delete' = 'teal';
  @Input() customClass: string = '';
}
