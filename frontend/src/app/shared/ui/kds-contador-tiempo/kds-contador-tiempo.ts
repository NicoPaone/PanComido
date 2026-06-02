import { Component, computed, input, signal } from '@angular/core';

@Component({
  selector: 'app-kds-contador-tiempo',
  imports: [],
  template: `{{ tiempoFormateado() }}`,
  styleUrl: './kds-contador-tiempo.css',
})
export class KdsContadorTiempo {

  fechaInicio = input.required<string>();

  private tick = signal(0);
  private intervaloId: ReturnType<typeof setInterval> | null = null;

  constructor(){
    this.intervaloId = setInterval(() => {
      this.tick.update(t => t + 1);
    }, 1000);
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
  ngOnDestroy() {
    if (this.intervaloId) {
      clearInterval(this.intervaloId);
    }
  }
}
