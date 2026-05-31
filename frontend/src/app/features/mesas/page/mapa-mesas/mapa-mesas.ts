import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { MesaStateService } from '../../services/mesa.state';
import { MesaItem } from '../../components/mesa-item/mesa-item';
import { EstadoMesa, FormaMesa } from '../../../../core/models/mesa.model';

@Component({
  selector: 'app-mapa-mesas',
  standalone: true,
  imports: [CommonModule, DragDropModule, MesaItem],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './mapa-mesas.html',
  styleUrl: './mapa-mesas.css'
})
export class MapaMesas implements OnInit {
  state = inject(MesaStateService);
  FormaMesa = FormaMesa;
  ngOnInit() {
    this.state.cargarMesas(); // Dispara la carga inicial al mock
  }

  gridSize = 15; // El tamaño de tu grilla en píxeles

  // Función que fuerza a la mesa a moverse visualmente en saltos de 15px


  onDragEnded(id: number, event: CdkDragEnd) {
    // También ajustamos la matemática acá para que el Signal guarde el múltiplo exacto
    const deltaX = Math.round(event.distance.x / this.gridSize) * this.gridSize;
    const deltaY = Math.round(event.distance.y / this.gridSize) * this.gridSize;

    if (deltaX === 0 && deltaY === 0) return;

    event.source._dragRef.reset();
    this.state.moverMesa(id, deltaX, deltaY);
  }
  ejecutarAccion(id: number, accion: string) {
    switch (accion) {
      case 'ocupar':
        this.state.cambiarEstadoMesa(id, EstadoMesa.Ocupada);
        break;
      case 'cerrar':
        this.state.cambiarEstadoMesa(id, EstadoMesa.Disponible);
        break;
      case 'deshabilitar':
        this.state.cambiarEstadoMesa(id, EstadoMesa.Deshabilitada);
        break;
      case 'detalles':
        // Por ahora lo dejamos vacío con un log, acá vamos a abrir el modal de la comanda después
        console.log('Próximamente: Abrir modal de detalles para la mesa', id);
        this.state.seleccionarMesa(null); // Solo cerramos el menú
        break;
    }
  } onMesaClick(id: number) {
    if (this.state.isEditorMode()) return; // En modo editor no abrimos modales


  }


}
