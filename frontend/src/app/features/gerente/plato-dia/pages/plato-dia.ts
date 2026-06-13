import { Component, ChangeDetectionStrategy, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlatoDiaStateService } from '../services/plato-dia.state';
import { AuthService } from '../../../../core/services/auth.service';
import { ArsCurrencyPipe } from '../../../../shared/pipes/ars-currency.pipe';
import { Plato } from '../../../../core/models/domain/plato';

@Component({
  selector: 'app-plato-dia',
  standalone: true,
  imports: [CommonModule, FormsModule, ArsCurrencyPipe],
  templateUrl: './plato-dia.html',
  styleUrls: ['./plato-dia.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlatoDiaComponent implements OnInit {
  readonly state = inject(PlatoDiaStateService);
  readonly authService = inject(AuthService);

  readonly platoSeleccionadoParaPrecio = signal<Plato | null>(null);
  readonly precioEspecialInput = signal<number>(0);

  // Estado del bloqueo de vista
  readonly vistaBloqueada = signal<boolean>(true);

  // Estado del Tutorial
  readonly pasoTutorial = signal<number | null>(null);

  readonly pasosTutorial = [
    {
      titulo: '1. Ingredientes Críticos',
      descripcion: 'Aquí visualizas los insumos con vencimientos próximos. Tienen alarmas visuales por criticidad: rojo para vencimiento de hoy o vencido, naranja para vencimiento pronto (2-3 días) y azul para vencimiento normal.'
    },
    {
      titulo: '2. Buscador y Filtros',
      descripcion: 'Filtra las sugerencias buscando platos por nombre, segmentando por categorías específicas de la carta, o aislando solo los platos que usan insumos de máxima urgencia.'
    },
    {
      titulo: '3. Acciones en Lote',
      descripcion: 'Ahorra tiempo configurando un porcentaje de descuento rápido para aplicarlo en lote a todas las sugerencias visibles en pantalla.'
    },
    {
      titulo: '4. Sugerencia por Impacto',
      descripcion: 'Los platos recomendados se ordenan automáticamente según su score de impacto en el inventario. Haz clic en "Seleccionar" en una tarjeta para establecer un precio especial y ver la calculadora de margen de ganancia.'
    },
    {
      titulo: '5. Confirmar Promociones',
      descripcion: 'Revisa las promociones acumuladas en el carrito inferior. Haz clic en el botón principal para confirmar los cambios e implementarlos en el menú activo.'
    }
  ];

  // Señales para búsqueda y filtros
  readonly busquedaNombre = signal<string>('');
  readonly categoriaSeleccionada = signal<string | null>(null);
  readonly urgenciaFiltro = signal<'todas' | 'criticas'>('todas');

  // Input para descuento en lote
  readonly descuentoLoteInput = signal<number>(20);

  // Listado de categorías disponibles calculadas a partir de los platos sugeridos
  readonly categoriasDisponibles = computed(() => {
    const sug = this.state.platosSugeridos();
    const cats = new Set<string>();
    sug.forEach(p => {
      if (p.categoria) cats.add(p.categoria);
    });
    return Array.from(cats);
  });

  // Platos sugeridos filtrados reactivamente por búsqueda, categoría y urgencia
  readonly platosSugeridosFiltrados = computed(() => {
    let sug = this.state.platosSugeridos();
    const query = this.busquedaNombre().toLowerCase().trim();
    const cat = this.categoriaSeleccionada();
    const filterUrgency = this.urgenciaFiltro();

    if (query) {
      sug = sug.filter(p => p.nombre.toLowerCase().includes(query) || p.descripcion?.toLowerCase().includes(query));
    }

    if (cat) {
      sug = sug.filter(p => p.categoria === cat);
    }

    if (filterUrgency === 'criticas') {
      sug = sug.filter(p => 
        p.ingredientesExpiring && p.ingredientesExpiring.some((ing: any) => 
          ing.estadoUrgencia === 'vencido' || ing.estadoUrgencia === 'hoy'
        )
      );
    }

    return sug;
  });

  // Cantidad de platos del listado filtrado que ya se encuentran en el carrito
  readonly totalFiltradosEnCarrito = computed(() => {
    const ids = new Set(this.platosSugeridosFiltrados().map(p => p.id));
    const seleccionados = this.state.promocionesSeleccionadas();
    return seleccionados.filter(s => ids.has(s.platoId)).length;
  });

  ngOnInit(): void {
    this.state.cargarDatos();
  }

  aplicarDescuentoLote(): void {
    const filtrados = this.platosSugeridosFiltrados();
    const desc = this.descuentoLoteInput();
    if (filtrados.length > 0 && desc > 0) {
      this.state.seleccionarPlatosEnLote(filtrados, desc);
    }
  }

  limpiarDescuentoLote(): void {
    const ids = this.platosSugeridosFiltrados().map(p => p.id);
    this.state.limpiarSeleccionadosEnLote(ids);
  }

  logout(): void {
    if (confirm('¿Desea cerrar la sesión actual de la aplicación?')) {
      this.authService.logout();
    }
  }

  abrirPrecioEspecial(plato: Plato): void {
    this.platoSeleccionadoParaPrecio.set(plato);
    // Sugerimos un precio especial con un 20% de descuento por defecto, redondeado
    const precioSugerido = Math.round(plato.precioVenta * 0.8);
    this.precioEspecialInput.set(precioSugerido);
  }

  cancelarPrecioEspecial(): void {
    this.platoSeleccionadoParaPrecio.set(null);
  }

  obtenerPromoEnCarrito(platoId: number) {
    return this.state.promocionesSeleccionadas().find(p => p.platoId === platoId);
  }

  confirmarPrecioEspecial(): void {
    const plato = this.platoSeleccionadoParaPrecio();
    const precio = this.precioEspecialInput();
    if (plato && precio > 0) {
      this.state.seleccionarPlato(plato, precio);
      this.platoSeleccionadoParaPrecio.set(null);
    }
  }

  confirmarPlatosDia(): void {
    this.state.confirmarPromociones();
  }

  quitarPromocion(platoId: number): void {
    if (confirm('¿Desea quitar la promoción de este plato? Volverá a su precio de carta habitual.')) {
      this.state.quitarPromocion(platoId);
    }
  }

  deseleccionarPlato(platoId: number): void {
    this.state.deseleccionarPlato(platoId);
  }

  // Métodos del tutorial
  iniciarTutorial(): void {
    this.pasoTutorial.set(0);
  }

  siguientePaso(): void {
    const pasoActual = this.pasoTutorial();
    if (pasoActual !== null && pasoActual < this.pasosTutorial.length - 1) {
      this.pasoTutorial.set(pasoActual + 1);
    } else {
      this.cerrarTutorial();
    }
  }

  anteriorPaso(): void {
    const pasoActual = this.pasoTutorial();
    if (pasoActual !== null && pasoActual > 0) {
      this.pasoTutorial.set(pasoActual - 1);
    }
  }

  cerrarTutorial(): void {
    this.pasoTutorial.set(null);
  }
}
