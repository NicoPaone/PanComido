import { Component, computed, input, output } from '@angular/core';
import { faBell, faCheck, faSpinner } from '@fortawesome/free-solid-svg-icons';
import { Llamado } from '../../../../core/models/llamados/llamado';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgClass } from '@angular/common';
import { IconProp } from '@fortawesome/fontawesome-svg-core';

@Component({
  selector: 'app-llamado-card',
  imports: [NgClass, FontAwesomeModule],
  templateUrl: './llamado-card.html',
  styleUrl: './llamado-card.css',
})
export class LlamadoCard {
  llamado = input.required<Llamado>();
  resolver = output<number>();
  resolviendo = input<boolean>(false);
  nuevo = input<boolean>(false);

  readonly iconoLlamada: IconProp = faBell;
  readonly iconoCheck: IconProp = faCheck;
  readonly iconoSpinner: IconProp = faSpinner;

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
    'es-nuevo': this.nuevo(),
  }));
}
