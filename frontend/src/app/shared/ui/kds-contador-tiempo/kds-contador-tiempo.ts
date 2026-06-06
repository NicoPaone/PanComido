import { Component, computed, input, signal , ChangeDetectionStrategy, inject, NgZone, DestroyRef} from '@angular/core';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-kds-contador-tiempo',
  imports: [],
  template: `{{ tiempoFormateado() }}`,
  styleUrl: './kds-contador-tiempo.css',
})
export class KdsContadorTiempo {

  fechaInicio = input.required<string>();

  private tick = signal(0);
  private ngZone = inject(NgZone);
  private destroyRef = inject(DestroyRef);

  constructor(){
    this.ngZone.runOutsideAngular(() => {
      const intervaloId = setInterval(() => {
        this.tick.update(t => t + 1);
      }, 1000);

      this.destroyRef.onDestroy(() => clearInterval(intervaloId));
    });
  }
  
  tiempoFormateado = computed(() => {
    this.tick();

    const inicio = new Date(this.fechaInicio()).getTime();
    const diferenciaMs = Date.now() - inicio;

    if (diferenciaMs < 0) return '00:00:00';

    const total = Math.floor(diferenciaMs / 1000);
    const h = String(Math.floor(total / 3600)).padStart(2, '0');
    const m = String(Math.floor((total % 3600) / 60)).padStart(2, '0');
    const s = String(total % 60).padStart(2, '0');

    return `${h}:${m}:${s}`;
  });
}
