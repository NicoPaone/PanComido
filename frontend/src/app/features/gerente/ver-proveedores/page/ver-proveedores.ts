import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark } from '@fortawesome/free-solid-svg-icons';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { PedidoProveedorItem, EstadoPedidoProveedor, Proveedor } from '../../../../core/models/proveedor';
import { ProveedorService } from '../../../../core/services/proveedor.service';
import { ProveedorListComponent } from '../components/proveedor-list/proveedor-list';
import { ProductoStockMock, UnidadMedida } from '../../../../core/model/producto-stock-mock';

@Component({
  selector: 'app-ver-proveedores',
  standalone: true,
  imports: [CommonModule, FormsModule, FontAwesomeModule, Buscador, Boton, Dropdown, PageToolbar, ProveedorListComponent],
  templateUrl: './ver-proveedores.html',
  styleUrl: './ver-proveedores.css'
})
export class VerProveedoresComponent implements OnInit {
  private readonly proveedorService = inject(ProveedorService);

  termino = signal('');
  proveedores = signal<Proveedor[]>([]);
  productos = signal<ProductoStockMock[]>([]);
  proveedorSeleccionadoId = signal<number | null>(null);
  panelModo = signal<'pedido' | 'historial'>('historial');
  observacionPedido = signal('');
  mensajeAccion = signal<string | null>(null);
  productoTexto = signal('');
  productoSeleccionadoId = signal<string | null>(null);
  cantidadProducto = signal<number | null>(1);
  pedidoItems = signal<PedidoProveedorItem[]>([]);
  faCheck = faCheck;
  faXmark = faXmark;

  proveedoresFiltrados = computed(() => {
    const texto = this.termino().toLowerCase().trim();
    const lista = [...this.proveedores()].sort((a, b) => {
      const fechaA = a.fechaUltimoPedido ? new Date(a.fechaUltimoPedido).getTime() : 0;
      const fechaB = b.fechaUltimoPedido ? new Date(b.fechaUltimoPedido).getTime() : 0;
      return fechaB - fechaA;
    });

    if (!texto) {
      return lista;
    }

    return lista.filter(proveedor => proveedor.nombre.toLowerCase().includes(texto));
  });

  productosFiltrados = computed(() => {
    const texto = this.productoTexto().toLowerCase().trim();
    const lista = [...this.productos()];

    if (!texto) {
      return lista;
    }

    return lista.filter(producto => producto.nombre.toLowerCase().includes(texto));
  });

  productoBaseActual = computed(() => {
    const productoSeleccionado = this.productos().find(producto => producto.id === this.productoSeleccionadoId());
    if (productoSeleccionado) {
      return productoSeleccionado;
    }

    const textoNormalizado = this.productoTexto().toLowerCase().trim();
    if (!textoNormalizado) {
      return null;
    }

    const coincidencias = this.productosFiltrados();
    const exacto = coincidencias.find(producto => producto.nombre.toLowerCase() === textoNormalizado);

    if (exacto) {
      return exacto;
    }

    return coincidencias.length === 1 ? coincidencias[0] : null;
  });

  proveedorSeleccionado = computed(() => {
    const proveedorId = this.proveedorSeleccionadoId();
    if (proveedorId === null) {
      return this.proveedoresFiltrados()[0] ?? null;
    }

    return this.proveedores().find(proveedor => proveedor.id === proveedorId) ?? null;
  });

  ngOnInit(): void {
    this.proveedorService.getProveedores().subscribe(proveedores => {
      this.proveedores.set(proveedores);

      if (proveedores.length > 0 && this.proveedorSeleccionadoId() === null) {
        this.proveedorSeleccionadoId.set(proveedores[0].id);
      }
    });

    this.proveedorService.getProductosDisponibles().subscribe(productos => {
      this.productos.set(productos);
    });
  }

  seleccionarProveedor(proveedor: Proveedor): void {
    this.proveedorSeleccionadoId.set(proveedor.id);
    this.mensajeAccion.set(null);
  }

  abrirPedido(proveedor: Proveedor): void {
    this.seleccionarProveedor(proveedor);
    this.panelModo.set('pedido');
    this.mensajeAccion.set(null);
  }

  abrirHistorial(proveedor: Proveedor): void {
    this.seleccionarProveedor(proveedor);
    this.panelModo.set('historial');
  }

  cambiarProveedorDesdePedido(proveedor: Proveedor, dropdown?: Dropdown): void {
    this.proveedorSeleccionadoId.set(proveedor.id);
    this.panelModo.set('pedido');
    this.limpiarPedido();
    dropdown?.cerrar();
    this.mensajeAccion.set(null);
  }

  seleccionarProducto(producto: ProductoStockMock): void {
    this.productoSeleccionadoId.set(producto.id);
    this.productoTexto.set(producto.nombre);
    this.cantidadProducto.set(this.getCantidadInicial(producto.unidadMedida));
  }

  onProductoTextoChange(valor: string): void {
    this.productoTexto.set(valor);
    const valorNormalizado = valor.toLowerCase().trim();
    const encontrado = this.productos().find(producto => producto.nombre.toLowerCase() === valorNormalizado);

    if (encontrado) {
      this.productoSeleccionadoId.set(encontrado.id);
      this.cantidadProducto.set(this.getCantidadInicial(encontrado.unidadMedida));
    } else {
      this.productoSeleccionadoId.set(null);
    }
  }

  agregarItemPedido(): void {
    const producto = this.productoBaseActual();
    const nombre = producto?.nombre ?? this.productoTexto().trim();
    const cantidad = this.cantidadProducto();

    if (!nombre || cantidad === null || cantidad <= 0) {
      return;
    }

    const unidadMedida = producto?.unidadMedida ?? 'UN';
    const itemId = producto?.id ?? `manual-${nombre.toLowerCase().replace(/\s+/g, '-')}`;

    this.pedidoItems.update(items => {
      const index = items.findIndex(item => item.id === itemId);

      if (index > -1) {
        return items.map(item => item.id === itemId ? { ...item, cantidad: item.cantidad + cantidad } : item);
      }

      return [...items, { id: itemId, nombre, cantidad, unidadMedida }];
    });

    this.productoTexto.set('');
    this.productoSeleccionadoId.set(null);
    this.cantidadProducto.set(1);
  }

  actualizarCantidadItem(itemId: string, cantidad: number | null): void {
    if (cantidad === null || cantidad <= 0) {
      return;
    }

    this.pedidoItems.update(items =>
      items.map(item => item.id === itemId ? { ...item, cantidad } : item)
    );
  }

  eliminarItemPedido(itemId: string): void {
    this.pedidoItems.update(items => items.filter(item => item.id !== itemId));
  }

  limpiarPedido(): void {
    this.pedidoItems.set([]);
    this.productoTexto.set('');
    this.productoSeleccionadoId.set(null);
    this.cantidadProducto.set(1);
    this.observacionPedido.set('');
    this.mensajeAccion.set(null);
  }

  enviarPedido(): void {
    const proveedor = this.proveedorSeleccionado();

    if (!proveedor || this.pedidoItems().length === 0) {
      return;
    }

    const pedido = {
      proveedorId: proveedor.id,
      concepto: 'Pedido de insumos',
      monto: this.calcularMontoEstimado(),
      observacion: this.observacionPedido().trim() || 'Pedido generado desde la vista de gerente',
      items: this.pedidoItems()
    };

    this.proveedorService.crearPedidoProveedor(proveedor.id, pedido).subscribe(actualizado => {
      this.proveedores.update(lista => lista.map(item => item.id === actualizado.id ? actualizado : item));
      this.proveedorSeleccionadoId.set(actualizado.id);
      this.panelModo.set('historial');
      this.limpiarPedido();
      this.mensajeAccion.set('Pedido agregado correctamente');
    });
  }

  getEstadoClase(estado: EstadoPedidoProveedor): string {
    switch (estado) {
      case 'Recibido':
        return 'estado-recibido';
      case 'Confirmado':
        return 'estado-confirmado';
      case 'Cancelado':
        return 'estado-cancelado';
      default:
        return 'estado-pendiente';
    }
  }

  get totalPedidosSeleccionado(): number {
    return this.proveedorSeleccionado()?.historialPedidos.length ?? 0;
  }

  get proveedorSeleccionadoPedido(): Proveedor | null {
    return this.proveedorSeleccionado();
  }

  get montoEstimado(): number {
    return this.calcularMontoEstimado();
  }

  getCantidadConfiguracion(unidadMedida: UnidadMedida): { step: number; min: number; placeholder: string } {
    switch (unidadMedida) {
      case 'UN':
        return { step: 1, min: 1, placeholder: '1' };
      case 'GR':
        return { step: 10, min: 10, placeholder: '100' };
      case 'KG':
      case 'L':
      default:
        return { step: 0.1, min: 0.1, placeholder: '0.5' };
    }
  }

  get productoSeleccionadoActual(): ProductoStockMock | null {
    return this.productos().find(producto => producto.id === this.productoSeleccionadoId()) ?? null;
  }

  get cantidadPasoProducto(): number {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').step;
  }

  get cantidadMinimaProducto(): number {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').min;
  }

  get cantidadPlaceholderProducto(): string {
    return this.getCantidadConfiguracion(this.productoBaseActual()?.unidadMedida ?? 'KG').placeholder;
  }

  private getCantidadInicial(unidadMedida: UnidadMedida): number {
    return this.getCantidadConfiguracion(unidadMedida).min;
  }

  private calcularMontoEstimado(): number {
    const precios: Record<string, number> = {
      '1': 1200,
      '2': 900,
      '3': 1500,
      '4': 600,
      '5': 1100,
      '6': 7500,
      '7': 120,
      '8': 300,
      '9': 800,
      '10': 700,
      '11': 4500
    };

    return this.pedidoItems().reduce((total, item) => {
      const base = precios[item.id] ?? 500;
      return total + base * item.cantidad;
    }, 0);
  }
}