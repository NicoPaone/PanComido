import { Component, inject, OnInit , ChangeDetectionStrategy, ChangeDetectorRef} from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { configuracionRestauranteMock } from '../../../../infra/mocks/configuracion-restaurante.mock-data';
import { PedidoState } from '../../services/pedido.state';
import { ItemPedido } from '../../../../core/models/domain/item-pedido';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { ComensalState } from '../../services/comensal-state';
import { ComandaState } from '../../services/comanda-state';
import { PlatoService } from '../../services/plato.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-personalizar-plato',
  standalone: true,
  imports: [FormsModule, DecimalPipe, LlamarAlMozo],
  templateUrl: './personalizar-plato.html',
  styleUrls: ['./personalizar-plato.css']
})
export class PersonalizarPlato implements OnInit {
  private router = inject(Router);
  private pedidoService = inject(PedidoState);
  private platoService = inject(PlatoService);
  private cdr = inject(ChangeDetectorRef);
  comensalState = inject(ComensalState);
  comandaState = inject(ComandaState);

  plato: ItemPedido | null = null;
  itemIndex: number = -1;
  configuracion = configuracionRestauranteMock;
  ingredientesExtra: string[] = [];
  ingredientesRemover: string[] = [];
  extrasSeleccionados: string[] = [];
  removidosSeleccionados: string[] = [];
  observaciones = '';

  ngOnInit() {
    const state = history.state;

    this.plato = state?.plato ?? null;
    this.itemIndex = state?.index ?? -1;

    const platoId = state?.plato?.plato?.articuloId;
    if (!platoId) return;

    this.platoService.getPlatoDetalle(platoId).subscribe(data => {
      console.log('Respuesta del plato:', data);
      console.log('Ingredientes:', data.ingredientes);

      // Usar sólo ingredientes opcionales: el comensal sólo puede agregarlos o sacarlos
      const opcionales = data.ingredientes
        .filter(i => i.opcional)
        .map(i => i.nombre);

      console.log('Ingredientes opcionales filtrados:', opcionales);

      this.ingredientesExtra = [...opcionales];
      this.ingredientesRemover = [...opcionales];

      if (opcionales.length === 0) {
        console.warn('No hay ingredientes opcionales para este plato');
      }

      // Notificar al change detector en OnPush mode
      this.cdr.markForCheck();

      // Precargar observaciones existentes si el ItemPedido ya venía en el estado
      if (this.plato) {
        this.observaciones = this.plato.observacionesGenerales ?? '';
        if (this.plato.observacionesIngredientes) {
          const parts = this.plato.observacionesIngredientes.split(', ').filter(Boolean);
          this.extrasSeleccionados = parts.filter(p => p.startsWith('+ ')).map(p => p.slice(2));
          this.removidosSeleccionados = parts.filter(p => p.startsWith('- ')).map(p => p.slice(2));
        }
      }
    });
  }

  volver() {
    this.router.navigate(['/comensal/pedido']);
  }

  toggleExtra(ingrediente: string) {
    if (this.extrasSeleccionados.includes(ingrediente)) {
      this.extrasSeleccionados = this.extrasSeleccionados.filter(i => i !== ingrediente);
    } else {
      this.extrasSeleccionados.push(ingrediente);
    }
    this.cdr.markForCheck();
  }

  toggleRemover(ingrediente: string) {
    if (this.removidosSeleccionados.includes(ingrediente)) {
      this.removidosSeleccionados = this.removidosSeleccionados.filter(i => i !== ingrediente);
    } else {
      this.removidosSeleccionados.push(ingrediente);
    }
    this.cdr.markForCheck();
  }

  guardarCambios() {
    if (this.itemIndex === -1) return;

    const ingredientes = [
      ...this.extrasSeleccionados.map(e => `+ ${e}`),
      ...this.removidosSeleccionados.map(r => `- ${r}`)
    ].join(', ');

    this.pedidoService.actualizarObservaciones(
      this.itemIndex,
      ingredientes,
      this.observaciones
    );

    this.router.navigate(['/comensal/pedido']);
  }
}
