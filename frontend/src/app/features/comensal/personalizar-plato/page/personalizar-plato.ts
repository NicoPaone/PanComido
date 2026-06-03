import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { configuracionRestauranteMock } from '../../../../core/interceptors/handlers/configuracion-restaurante.mock';
import { PedidoService } from '../../../../core/services/pedido.service';
import { ItemPedido } from '../../../../core/models/item-pedido';
import { LlamarAlMozo } from '../../components/llamar-al-mozo/llamar-al-mozo';
import { PedidoService } from '../../../../core/services/pedido.service';
import { PlatoService } from '../../../../core/services/plato.service';
import { ApiClient } from '../../../../core/services/api-client';
import { BotonComensal } from '../../../../shared/ui/botones/boton-comensal/boton-comensal';


@Component({
  selector: 'app-personalizar-plato',
  standalone: true,
<<<<<<< HEAD
  imports: [
    CommonModule,
    Boton,
    FormsModule,
    LlamarAlMozo,
    BotonComensal
  ],
=======
  imports: [FormsModule, DecimalPipe, LlamarAlMozo],
>>>>>>> 8ce2bf40f4a5cad7311dfe784aa30df3c8dad8f5
  templateUrl: './personalizar-plato.html',
  styleUrls: ['./personalizar-plato.css']
})
export class PersonalizarPlato implements OnInit {
  private router = inject(Router);
  private pedidoService = inject(PedidoService);

<<<<<<< HEAD
  plato: any;
=======
  plato: ItemPedido | null = null;
  itemIndex: number = -1;
>>>>>>> 8ce2bf40f4a5cad7311dfe784aa30df3c8dad8f5
  configuracion = configuracionRestauranteMock;

  

  ingredientesRemover: string[] = [];

  extrasSeleccionados: string[] = [];
  removidosSeleccionados: string[] = [];
  observaciones = '';

<<<<<<< HEAD
  constructor(
    private router: Router,
    private pedidoService: PedidoService,
    private platoService: PlatoService
    , private api: ApiClient
  ) {}

ngOnInit() {

  this.plato = history.state?.plato;

  console.log('PLATO:', this.plato);

  // Intentar obtener ingredientes desde el objeto (mocks) primero
  this.ingredientesRemover =
    this.plato?.plato?.receta?.map((i: any) => i.nombre) || [];

  // Si no hay receta (por ejemplo cuando la carta viene desde la API), pedir detalle del artículo
  if ((!this.ingredientesRemover || this.ingredientesRemover.length === 0) && this.plato?.plato?.id) {
    this.api.get<any>(`articulo/${this.plato.plato.id}`).subscribe(det => {
      const opciones = det?.ingredientesOpcionales ?? det?.IngredientesOpcionales ?? [];
      this.ingredientesRemover = opciones.map((o: any) => o?.nombre ?? o?.Nombre ?? o?.nombreIngrediente ?? 'Ingrediente');
    }, err => {
      console.warn('No se pudo obtener detalle del artículo', err);
    });
=======
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
>>>>>>> 8ce2bf40f4a5cad7311dfe784aa30df3c8dad8f5
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

<<<<<<< HEAD
    this.router.navigate(['/comensal/pedido']);
=======
    this.pedidoService.actualizarObservaciones(
      this.itemIndex,
      ingredientes,
      this.observaciones
    );
>>>>>>> 8ce2bf40f4a5cad7311dfe784aa30df3c8dad8f5

    this.router.navigate(['/comensal/pedido']);
  }
}
