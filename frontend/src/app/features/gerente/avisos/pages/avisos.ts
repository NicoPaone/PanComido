import { Component, ChangeDetectionStrategy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { take } from 'rxjs';

// Componentes UI
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';

// Modelos y Estados
import { Aviso } from '../../../../core/models/aviso.model';
import { VencimientosStateService } from '../../aviso-vencimientos/services/vencimientos.state';
import { AvisosStateService } from '../services/avisos.state';
import { UnidadMedida } from '../../../../core/models/unidad-medida';

@Component({
  selector: 'app-avisos',
  standalone: true,
  imports: [CommonModule, PageToolbar, Boton, Buscador],
  templateUrl: './avisos.html',
  styleUrls: ['./avisos.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AvisosPage implements OnInit {
  
  state = inject(AvisosStateService);
  pedidoState = inject(VencimientosStateService);
  router = inject(Router);
  
  isPedidoOffcanvasOpen = false;
  cantidadAgregar = 1;
  stockAvisoSeleccionado: Aviso | null = null;

  ngOnInit(): void {
    this.state.cargarSugerenciasCocina();
  }

  onBuscar(term: string) {
    this.state.setSearchTerm(term);
  }

  abrirAviso(aviso: Aviso) {
    this.abrirVencimientos(aviso.id);
  }

  abrirVencimientos(avisoId: string) {
    this.router.navigate(['/staff/gerente/aviso-vencimientos'], { queryParams: { avisoId } });
  }

  abrirPedidoStock(aviso: Aviso) {
    this.stockAvisoSeleccionado = aviso;
    this.cantidadAgregar = this.getCantidadInicial(aviso);
    
    // 🔥 EL FIX: Ahora pasamos objetos puros, sin inferencias extrañas
    this.pedidoState.seleccionarIngredienteParaPedido({
      id: Number(aviso.id), // Aseguramos que sea número si el estado lo exige
      nombre: aviso.titulo,
      fechaVencimiento: '', 
      stockDisponible: this.getStockDisponible(aviso),
      unidadMedida: this.getUnidadMedida(aviso)
    });
    
    this.isPedidoOffcanvasOpen = true;
  }

  cerrarPedidoStock() {
    this.isPedidoOffcanvasOpen = false;
    this.stockAvisoSeleccionado = null;
    this.pedidoState.limpiarSeleccion();
  }

  seleccionarProveedor(e: Event) {
    const proveedorId = (e.target as HTMLSelectElement).value;
    const proveedor = this.pedidoState.proveedoresDisponibles().find(item => item.id.toString() === proveedorId);
    if (proveedor) {
      this.pedidoState.seleccionarProveedor(proveedor);
    }
  }

  updateCantidad(e: Event) {
    this.cantidadAgregar = Number((e.target as HTMLInputElement).value);
  }

  confirmarPedidoStock() {
    const pedido = this.pedidoState.crearPedidoPendiente(this.cantidadAgregar);
    const aviso = this.stockAvisoSeleccionado;
    
    if (!pedido || !aviso) return;

    pedido.pipe(take(1)).subscribe({
      next: (proveedor) => {
        this.state.marcarRevisado(aviso.tipo, aviso.id);
        this.cerrarPedidoStock();
        this.router.navigate(['/staff', 'gerente', 'ver-proveedores', proveedor.id, 'historial']);
      },
      error: (err) => console.error('Error al confirmar pedido', err)
    });
  }

  cerrarConfirmacionCarta(): void {
    this.state.cerrarConfirmacionCarta();
  }

  irAModificarCarta(): void {
    this.state.cerrarConfirmacionCarta();
    this.router.navigate(['/staff', 'gerente', 'modificar-carta']);
  }

  // ⚠️ DEUDA TÉCNICA: El modelo Aviso debería traer el 'stockActual' numérico puro.
  // Parsear texto de la UI es peligroso.
  private getStockDisponible(aviso: Aviso): number {
    if (!aviso.subtitulo) return 0;
    const match = aviso.subtitulo.match(/[\d.]+/);
    return match ? Number(match[0]) : 0; // Cambiado a 0 por defecto, inventar un 1 falsea el stock.
  }

  // 🔥 EL FIX: Devuelve el objeto completo macheando con los IDs de tu backend
  private getUnidadMedida(aviso: Aviso): UnidadMedida {
    const subtitulo = aviso.subtitulo?.toUpperCase() || '';
    
    if (subtitulo.includes(' L'))  return { id: 3, nombre: 'Lt' };
    if (subtitulo.includes(' UN')) return { id: 5, nombre: 'Unidad' };
    if (subtitulo.includes(' GR')) return { id: 2, nombre: 'Gr' };
    
    return { id: 1, nombre: 'Kg' }; // Default
  }

  private getCantidadInicial(aviso: Aviso): number {
    const unidad = this.getUnidadMedida(aviso);
    // Si necesitas lógica específica por tipo de unidad, usas el nombre o el ID
    return unidad.nombre === 'Unidad' ? 1 : 1; 
  }
}