import { CommonModule } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Boton } from '../../../../../shared/ui/botones/boton/boton';
import { Proveedor } from '../../../../../core/models/proveedor';

@Component({
  selector: 'app-proveedor-list',
  standalone: true,
  imports: [CommonModule, Boton],
  templateUrl: './proveedor-list.html',
  styleUrl: './proveedor-list.css'
})
export class ProveedorListComponent {
  proveedores = input.required<Proveedor[]>();
  proveedorSeleccionadoId = input<number | null>(null);

  seleccionar = output<Proveedor>();
  crearPedido = output<Proveedor>();
  verHistorial = output<Proveedor>();

  esSeleccionado(proveedorId: number): boolean {
    return this.proveedorSeleccionadoId() === proveedorId;
  }

  trackPorId(_: number, proveedor: Proveedor): number {
    return proveedor.id;
  }
}