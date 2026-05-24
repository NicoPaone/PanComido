import { Component, effect, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { ToggleComponent } from '../../../../../shared/ui/toggle/toggle';

@Component({
  selector: 'app-modal-editar-plato',
  standalone: true,
  imports: [CommonModule, FormsModule, Boton, ToggleComponent],
  templateUrl: './modal-editar-plato.html',
  styleUrls: ['./modal-editar-plato.css']
})
export class ModalEditarPlatoComponent {
  plato = input.required<Plato>();
  save = output<Partial<Plato>>();
  close = output<void>();

  nombre = signal('');
  precioVenta = signal<number | null>(null);
  costo = signal<number | null>(null);
  imagen = signal('');
  visible = signal(true);

  constructor() {
    effect(() => {
      const p = this.plato();
      if (p) {
        this.nombre.set(p.nombre);
        this.precioVenta.set(p.precioVenta);
        this.costo.set(p.costo);
        this.imagen.set(p.imagen);
        this.visible.set(p.visible);
      }
    });
  }

  onToggleVisible() {
    this.visible.update(v => !v);
  }

  onSave() {
    if (!this.nombre().trim() || this.precioVenta() === null || this.costo() === null) {
      return;
    }
    this.save.emit({
      nombre: this.nombre(),
      precioVenta: this.precioVenta()!,
      costo: this.costo()!,
      imagen: this.imagen(),
      visible: this.visible()
    });
  }

  onClose() {
    this.close.emit();
  }
}
