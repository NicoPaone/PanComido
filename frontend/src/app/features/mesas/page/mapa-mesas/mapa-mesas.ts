import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
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

  onDragEnded(id: number, event: CdkDragEnd) {
    // 1. Obtenemos cuántos píxeles se movió el mouse desde el punto inicial
    const deltaX = Math.round(event.distance.x);
    const deltaY = Math.round(event.distance.y);

    // Si le hizo click pero no la movió, no hacemos nada
    if (deltaX === 0 && deltaY === 0) return;

    // 2. MAGIA NEGRA DEL CDK: Reseteamos la transformación CSS que aplica la librería.
    // Si no hacemos esto, cuando Angular actualice el top/left, la mesa se va a desplazar al doble.
    event.source._dragRef.reset();

    // 3. Le pasamos el problema al servicio de estado
    this.state.moverMesa(id, deltaX, deltaY);
  }

  onMesaClick(id: number) {
    if (this.state.isEditorMode()) return; // En modo editor no abrimos modales

    console.log('Se hizo click en la mesa', id);
    // Acá más adelante vamos a abrir el modal de "Asignar Mozo" o "Cobrar"
  }
}
