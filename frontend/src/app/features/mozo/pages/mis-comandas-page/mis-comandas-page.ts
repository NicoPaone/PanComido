import {  Component, effect, inject, signal , ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { Comanda } from '../../../../core/models/comanda/comanda';
import { MozoComandaState } from '../../services/mozo-comanda-state';
import { ComandaMozoCard } from "../../components/comanda-mozo-card/comanda-mozo-card";
import { ComandaMozoDetalle } from "../../components/comanda-mozo-detalle/comanda-mozo-detalle";
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-mis-comandas-page',
  imports: [ComandaMozoCard, ComandaMozoDetalle],
  templateUrl: './mis-comandas-page.html',
  styleUrl: './mis-comandas-page.css',
})
export class MisComandasPage implements OnDestroy {
  ngOnDestroy() { this.state.desconectarHub(); }
  
  state = inject(MozoComandaState);
  auth = inject(AuthService);
  comandaSeleccionada = signal<Comanda | null>(null);

  constructor() {
    effect(() => {
      const comanda = this.state.comandas().find(
        c => c.id === this.comandaSeleccionada()?.id
      );
      if (comanda) {
        this.comandaSeleccionada.set(comanda);
      }
    });
  }

  ngOnInit(): void {
    this.state.cargarComandas();
    void this.state.conectarHub(this.auth.currentRestauranteId, this.auth.currentMozoId);
  }

  abrirDetalle(comandaId: number): void {
    const comanda = this.state.comandas().find(c => c.id === comandaId);
    if (comanda) {
      this.comandaSeleccionada.set(comanda);
    }
  }

  cerrarDetalle(): void {
    this.comandaSeleccionada.set(null);
  }

  onEntregar(evento: { comandaId: number; articuloComandaIds: number[] }): void {
    this.state.entregarItems(evento.comandaId, evento.articuloComandaIds);
    this.comandaSeleccionada.set(null);
  }
}
