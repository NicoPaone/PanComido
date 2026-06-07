import { CommonModule } from '@angular/common';
import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { Proveedor } from '../../../../../core/models/domain/proveedor';

@Component({
  selector: 'app-proveedor-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './proveedor-list.html',
  styleUrl: './proveedor-list.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProveedorListComponent {
  proveedores = input.required<Proveedor[]>();
  proveedorSeleccionadoId = input<number |string| null>(null);

  seleccionar = output<Proveedor>();
  crearPedido = output<Proveedor>();
  verHistorial = output<Proveedor>();

  esSeleccionado(proveedorId: number | string): boolean {
    return this.proveedorSeleccionadoId() === proveedorId;
  }

  trackPorId(_: number | string, proveedor: Proveedor): number | string {
    return proveedor.id;
  }

  contactoProveedor(proveedor: Proveedor): string {
    return proveedor.telefono?.trim() || 'Sin telefono registrado';
  }
}
