import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Buscador } from '../../../../shared/ui/buscador/buscador';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Plato } from '../../../../core/models/plato';
import { ListaPlatosComponent } from '../components/lista-platos/lista-platos';
import { BotonCategoriasComponent } from '../../../../shared/ui/botones/boton-categorias/boton-categorias';

@Component({
  selector: 'app-modificar-carta',
  standalone: true,
  imports: [CommonModule, Buscador, Boton, ListaPlatosComponent, BotonCategoriasComponent],
  templateUrl: './modificar-carta.html',
  styleUrls: ['./modificar-carta.css']
})
export class ModificarCartaComponent implements OnInit {

  platos: Plato[] = [
    {
      id: 1,
      nombre: 'Milanesa napolitana',
      descripcion: 'Milanesa de carne con salsa de tomate, jamón y mozzarella gratinada.',
      precioVenta: 16200,
      costo: 13160,
      tiempo: 25,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 2,
      nombre: 'Porción de papas',
      descripcion: 'Papas fritas crocantes acompañadas con salsa especial de la casa.',
      precioVenta: 10000,
      costo: 7000,
      tiempo: 15,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1576107232684-1279f390859f?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 3,
      nombre: 'Pasta al pesto',
      descripcion: 'Pasta artesanal con salsa pesto de albahaca fresca y queso parmesano.',
      precioVenta: 12600,
      costo: 8700,
      tiempo: 20,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 4,
      nombre: 'Pizza de muzarella',
      descripcion: 'Pizza clásica con abundante mozzarella y salsa de tomate casera.',
      precioVenta: 12600,
      costo: 8700,
      tiempo: 25,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 5,
      nombre: 'Pastel de papa',
      descripcion: 'Pastel casero de carne condimentada y puré de papas gratinado.',
      precioVenta: 14800,
      costo: 9320,
      tiempo: 30,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 6,
      nombre: 'Pollo al curry',
      descripcion: 'Pollo cocinado en salsa curry suave acompañado con arroz especiado.',
      precioVenta: 19500,
      costo: 8600,
      tiempo: 25,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: true,
      imagen: 'https://images.unsplash.com/photo-1565557612662-811c7504ee42?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 7,
      nombre: 'Solomillo de cerdo con salsa',
      descripcion: 'Solomillo de cerdo tierno servido con salsa cremosa y vegetales.',
      precioVenta: 19460,
      costo: 10120,
      tiempo: 45,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&q=80&w=200&h=150'
    },
    {
      id: 8,
      nombre: 'Risotto a la crema',
      descripcion: 'Risotto cremoso preparado con queso parmesano y hierbas frescas.',
      precioVenta: 29460,
      costo: 20120,
      tiempo: 30,
      tipo: 'principal',
  bebida: '',
  restriccion: '',
      visible: false,
      imagen: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=150'
    }
  ];
  filteredPlatos: Plato[] = [];

  ngOnInit() {
    this.filteredPlatos = [...this.platos];
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
}
