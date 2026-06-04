import { Component, inject, OnInit , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { PedidoState } from '../../services/pedido.state';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { ComensalState } from '../../services/comensal-state';
import { ComandaState } from '../../services/comanda-state';

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
  comensalState = inject(ComensalState);
  comandaState = inject(ComandaState);

  plato: ItemPedido | null = null;
  itemIndex: number = -1;
  configuracion = configuracionRestauranteMock;

  ingredientesExtra = [
    'Queso extra',
    'Panceta',
    'Huevo',
    'Palta',
    'Salsa picante'
  ];

  ingredientesRemover = [
    'Cebolla',
    'Tomate',
    'Mostaza',
    'Mayonesa'
  ];

  extrasSeleccionados: string[] = [];
  removidosSeleccionados: string[] = [];
  observaciones = '';

  ngOnInit() {
    const state = history.state;
    this.plato = state?.plato ?? null;
    this.itemIndex = state?.index ?? -1;

    // Precargar observaciones existentes
    if (this.plato) {
      this.observaciones = this.plato.observacionesGenerales ?? '';
      if (this.plato.observacionesIngredientes) {
        const parts = this.plato.observacionesIngredientes.split(', ').filter(Boolean);
        this.extrasSeleccionados = parts.filter(p => p.startsWith('+ ')).map(p => p.slice(2));
        this.removidosSeleccionados = parts.filter(p => p.startsWith('- ')).map(p => p.slice(2));
      }
    }
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
  }

  toggleRemover(ingrediente: string) {
    if (this.removidosSeleccionados.includes(ingrediente)) {
      this.removidosSeleccionados = this.removidosSeleccionados.filter(i => i !== ingrediente);
    } else {
      this.removidosSeleccionados.push(ingrediente);
    }
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
