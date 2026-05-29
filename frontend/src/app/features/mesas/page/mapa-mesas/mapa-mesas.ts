import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragEnd, DragDropModule, Point } from '@angular/cdk/drag-drop';
import { MesaStateService } from '../../services/mesa.state';
import { MesaItem } from '../../components/mesa-item/mesa-item';

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

  onMesaClick(id: number) {
    if (this.state.isEditorMode()) return; // En modo editor no abrimos modales

    console.log('Se hizo click en la mesa', id);
    // Acá más adelante vamos a abrir el modal de "Asignar Mozo" o "Cobrar"
  }


}
