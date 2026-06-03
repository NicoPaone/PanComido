import { Component, inject, input, signal } from '@angular/core';
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
import { LlamadoService } from '../../../../core/services/llamados/llamado-service';
import { LlamarMozoRequest } from '../../../../core/models/llamados/llamado';
import { ComandaStateService } from '../../services/comanda-state.service';


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
  selector: 'app-llamar-al-mozo',
  standalone: true,
  imports: [FormsModule, FontAwesomeModule],
  templateUrl: './llamar-al-mozo.html',
  styleUrls: ['./llamar-al-mozo.css'],
})
export class LlamarAlMozo {
  readonly #api = inject(LlamadoService);
  readonly #comandaState = inject(ComandaStateService);

  configuracion = input.required<any>();

  readonly categorias = CATEGORIAS;
  readonly categoriaSeleccionada = signal<number | null>(null);
  readonly descripcion = signal('');
  readonly enviando = signal(false);
  readonly modalAbierto = signal(false);
  readonly enviado = signal(false);
  readonly error = signal('');

  get mesaId(): number {
    return this.#comandaState.mesaId() ?? 0;
  }

  abrirModal(): void {
    this.categoriaSeleccionada.set(null);
    this.descripcion.set('');
    this.enviando.set(false);
    this.error.set('');
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
    this.enviado.set(false);
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

    this.enviando.set(true);

    const request: LlamarMozoRequest = {
      mesaId: this.mesaId,
      categoriaLlamadoId: categoriaId,
      descripcion: this.descripcion(),
    };

    this.#api.crearLlamado(request).subscribe({
      next: () => {
        this.enviando.set(false);
        this.enviado.set(true);
      },
      error: (err) => {
        this.enviando.set(false);
        this.error.set(err.error?.error ?? 'No se pudo enviar el llamado. Intenta nuevamente.');
      },
    });
  }
}
