import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Plato } from '../../../../../core/models/plato';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';

@Component({
  selector: 'app-modal-eliminar-plato',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './modal-eliminar-plato.html',
  styleUrls: ['./modal-eliminar-plato.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ModalEliminarPlatoComponent {
  plato = input.required<Plato>();
  confirm = output<void>();
  close = output<void>();

  onConfirm() {
    this.confirm.emit();
  }

  onClose() {
    this.close.emit();
  }
}
