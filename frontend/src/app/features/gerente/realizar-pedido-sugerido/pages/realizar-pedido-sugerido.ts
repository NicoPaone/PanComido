import { Component, OnInit, inject , ChangeDetectionStrategy} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Proveedor, SugerenciaPedidoItem } from '../../../../core/models/domain/proveedor';
import { UnidadMedida } from '../../../../core/models/domain/unidad-medida';
import { RealizarPedidoSugeridoStateService } from '../services/realizar-pedido-sugerido.state';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-realizar-pedido-sugerido',
  standalone: true,
  imports: [DecimalPipe, FormsModule, Boton, Buscador],
  templateUrl: './realizar-pedido-sugerido.html',
  styleUrls: ['./realizar-pedido-sugerido.css']
})
export class RealizarPedidoSugeridoComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly state = inject(RealizarPedidoSugeridoStateService);

  // Exponer señales del State Service para que la plantilla HTML y los tests sigan funcionando sin cambios
  proveedorId = this.state.proveedorId;
  proveedor = this.state.proveedor;
  proveedores = this.state.proveedores;
  sugerencias = this.state.sugerencias;
  pedidoItems = this.state.pedidoItems;
  observaciones = this.state.observaciones;
  busqueda = this.state.busqueda;
  busquedaProveedor = this.state.busquedaProveedor;
  mensajeError = this.state.mensajeError;
  loading = this.state.loading;

  montoEstimado = this.state.montoEstimado;
proveedoresFiltrados = this.state.proveedoresFiltrados;

  // TODO REFACTOR: panel "Agregar ingredientes" duplicado de historial-proveedor (03/06/2026)
  proveedorAgregarIngredienteId = this.state.proveedorAgregarIngredienteId;
  busquedaIngrediente = this.state.busquedaIngrediente;
  productoExtraSeleccionadoId = this.state.productoExtraSeleccionadoId;
  cantidadIngrediente = this.state.cantidadIngrediente;
  precioIngrediente = this.state.precioIngrediente;
  ingredientesParaAgregar = this.state.ingredientesParaAgregar;

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.state.proveedorId.set(+idParam);
    }
  }

  ngOnInit(): void {
    const id = this.state.proveedorId();
    this.state.cargarDatos(id > 0 ? id : undefined);
  }

  seleccionarProveedor(id: string | number): void {
    this.state.seleccionarProveedor(id);
  }

  onSearchChanged(query: string): void {
    this.state.setSearchTerm(query);
  }

  itemsProveedor(proveedorId: string | number): SugerenciaPedidoItem[] {
    return this.state.obtenerItemsProveedor(proveedorId);
  }

  montoProveedor(proveedorId: string | number): number {
    return this.state.calcularMontoProveedor(proveedorId);
  }

  observacionProveedor(proveedorId: string | number): string {
    return this.state.obtenerObservacionProveedor(proveedorId);
  }

  setObservacionProveedor(proveedorId: string | number, observacion: string): void {
    this.state.setObservacionProveedor(proveedorId, observacion);
  }

  eliminarItem(proveedorId: string | number, productoId: string): void {
    this.state.eliminarItem(proveedorId, productoId);
  }

  onCantidadCambiada(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    this.state.onCantidadCambiada(proveedorId, item, val);
  }

  onPrecioCambiado(proveedorId: string | number, item: SugerenciaPedidoItem, val: number | null): void {
    this.state.onPrecioCambiado(proveedorId, item, val);
  }

  nombreUnidad(unidadMedida: UnidadMedida | string | null | undefined): string {
    if (!unidadMedida) return '';
    return typeof unidadMedida === 'string' ? unidadMedida : unidadMedida.nombre;
  }

  totalItems(): number {
    return this.proveedoresFiltrados().reduce((total, proveedor) => total + this.itemsProveedor(proveedor.id).length, 0);
  }

  totalEstimado(): number {
    return this.proveedoresFiltrados().reduce((total, proveedor) => total + this.montoProveedor(proveedor.id), 0);
  }

  pedidosListos(): number {
    return this.proveedoresFiltrados().filter(proveedor => this.itemsProveedor(proveedor.id).length > 0).length;
  }

  cantidadDisplay(item: SugerenciaPedidoItem): number {
    return this.usaUnidadMenor(item) ? Math.round(item.cantidadSugerida * 1000) : item.cantidadSugerida;
  }

  setCantidadDisplay(proveedorId: string | number, item: SugerenciaPedidoItem, valor: number | string | null): void {
    if (valor === null || valor === '') {
      this.onCantidadCambiada(proveedorId, item, null);
      return;
    }

    const cantidad = Number(valor);
    if (!Number.isFinite(cantidad)) {
      this.onCantidadCambiada(proveedorId, item, null);
      return;
    }

    this.onCantidadCambiada(proveedorId, item, this.usaUnidadMenor(item) ? cantidad / 1000 : cantidad);
  }

  ajustarCantidadDisplay(proveedorId: string | number, item: SugerenciaPedidoItem, delta: number): void {
    const siguiente = Math.max(this.cantidadDisplay(item) + delta, this.cantidadMinimaDisplay(item));
    this.setCantidadDisplay(proveedorId, item, siguiente);
  }

  unidadDisplay(item: SugerenciaPedidoItem): string {
    if (this.esUnidadPeso(item)) return 'g';
    if (this.esUnidadGramos(item)) return 'gr';
    if (this.esUnidadVolumen(item)) return 'ml';
    return this.nombreUnidad(item.unidadMedida).toLowerCase() || 'un';
  }

  cantidadMinimaDisplay(item: SugerenciaPedidoItem): number {
    return this.usaUnidadMenor(item) || this.esUnidadGramos(item) ? 1 : 1;
  }

  cantidadPasoDisplay(item: SugerenciaPedidoItem): number {
    if (this.esUnidadGramos(item)) return 10;
    if (this.esUnidadPeso(item) || this.esUnidadVolumen(item)) return 100;
    return 1;
  }

  presetsCantidad(item: SugerenciaPedidoItem): Array<{ label: string; value: number }> {
    if (this.esUnidadPeso(item)) {
      return [
        { label: '100 g', value: 100 },
        { label: '250 g', value: 250 },
        { label: '500 g', value: 500 },
        { label: '1 kg', value: 1000 }
      ];
    }

    if (this.esUnidadGramos(item)) {
      return [
        { label: '10 gr', value: 10 },
        { label: '25 gr', value: 25 },
        { label: '50 gr', value: 50 },
        { label: '100 gr', value: 100 }
      ];
    }

    if (this.esUnidadVolumen(item)) {
      return [
        { label: '100 ml', value: 100 },
        { label: '250 ml', value: 250 },
        { label: '500 ml', value: 500 },
        { label: '1 l', value: 1000 }
      ];
    }

    return [];
  }

  equivalenciaCantidad(item: SugerenciaPedidoItem): string | null {
    if (this.esUnidadGramos(item)) {
      return this.cantidadDisplay(item) >= 1000
        ? `Equivale a ${this.formatearEquivalencia(this.cantidadDisplay(item) / 1000, 'kg', 'gr')}`
        : null;
    }

    if (!this.usaUnidadMenor(item) || item.cantidadSugerida <= 0) return null;
    return `Se agregara como ${this.formatearEquivalencia(item.cantidadSugerida, this.esUnidadPeso(item) ? 'kg' : 'l', this.esUnidadPeso(item) ? 'g' : 'ml')}`;
  }

  private usaUnidadMenor(item: SugerenciaPedidoItem): boolean {
    return this.esUnidadPeso(item) || this.esUnidadVolumen(item);
  }

  private esUnidadPeso(item: SugerenciaPedidoItem): boolean {
    const unidad = this.nombreUnidad(item.unidadMedida).trim().toUpperCase();
    return ['KG', 'KILO', 'KILOS'].includes(unidad);
  }

  private esUnidadGramos(item: SugerenciaPedidoItem): boolean {
    const unidad = this.nombreUnidad(item.unidadMedida).trim().toUpperCase();
    return ['G', 'GR', 'GRAMO', 'GRAMOS'].includes(unidad);
  }

  private esUnidadVolumen(item: SugerenciaPedidoItem): boolean {
    const unidad = this.nombreUnidad(item.unidadMedida).trim().toUpperCase();
    return ['L', 'LT', 'LITRO', 'LITROS'].includes(unidad);
  }

  private formatearEquivalencia(cantidad: number, unidadMayor: string, unidadMenor: string): string {
    const enteros = Math.trunc(cantidad);
    const menores = Math.round((cantidad - enteros) * 1000);
    if (enteros > 0 && menores > 0) return `${enteros} ${unidadMayor} ${menores} ${unidadMenor}`;
    if (enteros > 0) return `${enteros} ${unidadMayor}`;
    return `${Math.round(cantidad * 1000)} ${unidadMenor}`;
  }

  volver(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  enviarPedido(proveedor: Proveedor): void {
    this.state.enviarPedido(proveedor, () => {
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores', proveedor.id, 'historial'], {
        state: { created: true, message: 'Pedido creado correctamente. Quedó pendiente en el historial.' }
      });
    });
  }

  // TODO REFACTOR: panel "Agregar ingredientes" duplicado de historial-proveedor (03/06/2026)
  abrirAgregarIngrediente(proveedorId: number | string): void {
    this.state.abrirAgregarIngrediente(proveedorId);
  }

  cerrarAgregarIngrediente(): void {
    this.state.cerrarAgregarIngrediente();
  }

  seleccionarIngredienteExtra(productoId: string): void {
    this.state.seleccionarIngredienteExtra(productoId);
  }

  onCantidadIngredienteChange(val: number | null): void {
    this.state.setCantidadIngrediente(val);
  }

  onPrecioIngredienteChange(val: number | null): void {
    this.state.setPrecioIngrediente(val);
  }

  confirmarAgregarIngrediente(): void {
    this.state.confirmarAgregarIngrediente();
  }
}
