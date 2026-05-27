import { Component, computed, input, OnInit, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Plato } from '../../../../core/models/plato';
import { ListaPlatosComponent } from '../components/lista-platos/lista-platos';
import { PageToolbar } from '../../../../shared/ui/page-toolbar/page-toolbar';
import { Dropdown } from '../../../../shared/ui/dropdown/dropdown';
import { Modal } from '../../../../shared/ui/modal/modal';
import { FormPlatoEditar } from "../components/form-plato-editar/form-plato-editar";

@Component({
  selector: 'app-modificar-carta',
  standalone: true,
  imports: [CommonModule, Buscador, Boton, ListaPlatosComponent, Dropdown, PageToolbar, Modal, FormPlatoEditar],
  templateUrl: './modificar-carta.html',
  styleUrls: ['./modificar-carta.css']
})
export class ModificarCartaComponent implements OnInit {
  platos: Plato[] = [
    {
      id: 1,
      nombre: 'Milanesa napolitana',
      precioVenta: 16200,
      costo: 13160,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 2,
      nombre: 'Porción de papas',
      precioVenta: 10000,
      costo: 7000,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 3,
      nombre: 'Pasta al pesto',
      precioVenta: 12600,
      costo: 8700,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 4,
      nombre: 'Pizza de muzarella',
      precioVenta: 12600,
      costo: 8700,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 5,
      nombre: 'Pastel de papa',
      precioVenta: 14800,
      costo: 9320,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 6,
      nombre: 'Pollo al curry',
      precioVenta: 19500,
      costo: 8600,
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1565557612662-811c7504ee42?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 7,
      nombre: 'Solomillo de cerdo con salsa',
      precioVenta: 19460,
      costo: 10120,
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 8,
      nombre: 'Risotto a la crema',
      precioVenta: 29460,
      costo: 20120,
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=150'
    }
  ];
  
  
  ngOnInit() {
    this.filteredPlatos = [...this.platos];
  }
  
  filteredPlatos: Plato[] = [];
  tamanioMaximo = 500;
  tituloModal = "Editar plato"
  categorias = ['Principales', 'Entradas', 'Bebidas', 'Postres'];
  
  platoEditandoId = signal<number | null>(null);
  
  
  platoSeleccionado = computed(() => {
    const id = this.platoEditandoId();
    if (id === null || id === undefined) return null; 
    return this.platos.find(p => p.id === id) || null;
  });
  
  abrirModalEditar(modal:Modal, id: number){
    this.platoEditandoId.set(id);
    modal.abrir();
  }

  cerrarYLimpiar(modal: Modal){
    modal.cerrar();
    this.platoEditandoId.set(null);
  }


    

  onSearch(term: string) {
    const lowerTerm = term.toLowerCase().trim();
    if (!lowerTerm) {
      this.filteredPlatos = [...this.platos];
    } else {
      this.filteredPlatos = this.platos.filter(plato => 
        plato.nombre.toLowerCase().includes(lowerTerm)
      );
    }
  }

  toggleVisibility(plato: Plato) {
    plato.visible = !plato.visible;
  }

  
  onCategoriaSeleccionada(categoria: string) {
    console.log('Categoría seleccionada:', categoria);
    // TODO: Agregar lógica de filtro por categoría si el modelo de Plato tiene esa propiedad
  }
  
  guardarPlato(datosEditados: Partial<Plato>, modal: Modal) {
    const idEdicion = this.platoEditandoId();
    if (idEdicion) {
      // Actualizar plato existente
      const index = this.platos.findIndex(p => p.id === idEdicion);
      if (index !== -1) {
        this.platos[index] = { ...this.platos[index], ...datosEditados };
        console.log('Plato actualizado:', this.platos[index]);
      }
    }
    this.cerrarYLimpiar(modal);
  }
}
