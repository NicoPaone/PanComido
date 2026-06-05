import { Component, inject, input, signal, ChangeDetectionStrategy, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faSnowflake,
  faFlask,
  faUser,
  faUtensils,
  faPepperHot,
  faBreadSlice,
  IconDefinition,
} from '@fortawesome/free-solid-svg-icons';
import { LlamadoMozo } from '../../../../core/models/domain/llamado';


interface CategoriaLlamado {
  id: number;
  nombre: string;
  icono: IconDefinition;
}

const CATEGORIAS: CategoriaLlamado[] = [
  { id: 1, nombre: 'HIELO', icono: faSnowflake },
  { id: 2, nombre: 'SAL', icono: faFlask },
  { id: 3, nombre: 'GENERAL', icono: faUser },
  { id: 4, nombre: 'SERVILLETA', icono: faUtensils },
  { id: 5, nombre: 'CONDIMENTOS', icono: faPepperHot },
  { id: 6, nombre: 'PANERA', icono: faBreadSlice },
];

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-llamar-al-mozo',
  standalone: true,
  imports: [FormsModule, FontAwesomeModule],
  templateUrl: './llamar-al-mozo.html',
  styleUrls: ['./llamar-al-mozo.css'],
})
export class LlamarAlMozo {
  configuracion = input.required<any>();
  mesaId = input.required<number>();
  enviando = input<boolean>(false);
  enviado = input<boolean>(false);
  error = input<string | null>(null);

  llamadoMozo = output<LlamadoMozo>();
  modalCerrado = output<void>();

  readonly categorias = CATEGORIAS;
  readonly categoriaSeleccionada = signal<number | null>(null);
  readonly descripcion = signal('');
  readonly modalAbierto = signal(false);

  abrirModal(): void {
    this.categoriaSeleccionada.set(null);
    this.descripcion.set('');
    this.modalAbierto.set(true);

    setTimeout(() => {
      const dialog = document.getElementById('modal-llamar-mozo') as HTMLDialogElement;
      dialog?.showModal();
    });
  }

  cerrarModal(): void {
    const dialog = document.getElementById('modal-llamar-mozo') as HTMLDialogElement;
    dialog?.close();
    this.modalAbierto.set(false);
    this.modalCerrado.emit();
  }

  aceptar(): void {
    this.cerrarModal();
  }

  seleccionarCategoria(id: number): void {
    this.categoriaSeleccionada.set(id);
  }

  enviar(): void {
    const categoriaId = this.categoriaSeleccionada();
    if (!categoriaId || this.enviando()) return;

    this.llamadoMozo.emit({
      mesaId: this.mesaId(),
      categoriaLlamadoId: categoriaId,
      descripcion: this.descripcion(),
    });
  }
}
