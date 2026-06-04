import { Component, computed, input, output , ChangeDetectionStrategy} from '@angular/core';
import { Comanda } from '../../../../core/models/comanda/comanda';
import { KdsContadorTiempo } from "../../../../shared/ui/kds-contador-tiempo/kds-contador-tiempo";

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-comanda-mozo-card',
  imports: [KdsContadorTiempo],
  templateUrl: './comanda-mozo-card.html',
  styleUrl: './comanda-mozo-card.css',
})
export class ComandaMozoCard {

  comanda = input.required<Comanda>()
  ver = output<number>();

  readonly headerClass = computed(() => {
    const map: Record<string, string> = {
      'Nueva': 'bg-nueva',
      'EnPreparacion': 'bg-preparacion',
      'EnEspera': 'bg-espera',
    };
    return map[this.comanda().estado] ?? '';
  });

  readonly itemsVisibles = computed(() => {
    return this.comanda().items.slice(0,3);
  })
  readonly cantidadItems = computed(() => this.comanda().items.length);

  onVer(): void {
    this.ver.emit(this.comanda().id);
  }
}
