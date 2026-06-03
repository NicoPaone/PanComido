import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CdkDragEnd, DragDropModule } from '@angular/cdk/drag-drop';
import { MesaStateService } from '../../services/mesa.state';
import { EstadoMesa, FormaMesa } from '../../../../core/models/mesa.model';
import { MesaItem } from '../../../../shared/components/mesa-item/mesa-item';

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
        console.log('Próximamente: Abrir modal de detalles para la mesa', id);
        this.state.seleccionarMesa(null); 
        break;
    }
  }
  
  onMesaClick(id: number) {
    if (this.state.isEditorMode()) return; 
    
    // Próximamente: Llamar al backend para traer la comanda activa por MesaId
    // Ej: GET /comanda/mesa/{id}/activa
    console.log('Buscar comanda activa para mesa:', id);
  }

  mesasOrdenadas() {
    return [...this.state.mesas()].sort((a, b) => a.numeroMesa - b.numeroMesa);
  }

  getMobileClass(estado: string): string {
    switch (estado.toLowerCase()) {
      case 'disponible': return 'disponible';
      case 'ocupada': return 'ocupada';
      case 'reservada': return 'reservada';
      case 'deshabilitada': return 'deshabilitada';
      default: return 'disponible';
    }
  }


}
