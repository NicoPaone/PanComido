import { Component, computed, input, output , ChangeDetectionStrategy} from '@angular/core';
import {
  faSnowflake,
  faFlask,
  faUser,
  faUtensils,
  faPepperHot,
  faBreadSlice,
  faCheck,
  faSpinner,
  IconDefinition,
} from '@fortawesome/free-solid-svg-icons';
import { Llamado } from '../../../../core/models/llamados/llamado';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgClass } from '@angular/common';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

const ICONOS_CATEGORIA: Record<number, IconDefinition> = {
  1: faSnowflake,
  2: faFlask,
  3: faUser,
  4: faUtensils,
  5: faPepperHot,
  6: faBreadSlice,
};

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-llamado-card',
  imports: [NgClass, FontAwesomeModule],
  templateUrl: './llamado-card.html',
  styleUrl: './llamado-card.css',
})
export class LlamadoCard {
  llamado = input.required<Llamado>();
  resolver = output<number>();
  resolviendo = input<boolean>(false);
  saliendo = input<boolean>(false);
  nuevo = input<boolean>(false);

  readonly iconoCheck: IconProp = faCheck;
  readonly iconoSpinner: IconProp = faSpinner;

  readonly iconoCategoria = computed<IconProp>(() => {
    const id = this.llamado().categoriaLlamadoId;
    return (ICONOS_CATEGORIA[id] ?? faSnowflake) as IconProp;
  });

  readonly iconoAccion = computed<IconProp>(() =>
    this.resolviendo() ? this.iconoSpinner : this.iconoCheck,
  );

  readonly titulo = computed(() => {
    const mesa = this.llamado().mesaId ? `Mesa ${this.llamado().mesaId}` : '';
    return mesa;
  });

  readonly clases = computed<Record<string, boolean>>(() => ({
    'llamado-card': true,
    'esta-resolviendo': this.resolviendo(),
    'esta-saliendo': this.saliendo(),
    'es-nuevo': this.nuevo(),
  }));
}
