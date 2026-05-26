import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark, faTrash } from '@fortawesome/free-solid-svg-icons';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Router } from '@angular/router';
import { ProveedorService } from '../../../../core/services/proveedor.service';
import { NuevoProveedor } from '../../../../core/models/proveedor';

@Component({
  selector: 'app-nuevo-proveedor',
  standalone: true,
  imports: [CommonModule, FormsModule, FontAwesomeModule, Boton],
  templateUrl: './nuevo-proveedor.html',
  styleUrls: ['./nuevo-proveedor.css']
})
export class NuevoProveedorComponent {
  private readonly proveedorService = inject(ProveedorService);
  private readonly router = inject(Router);

  nuevoProveedor = signal<NuevoProveedor>({ nombre: '', contacto: '', telefono: '', email: '', calle: '', numero: '', ciudad: '', categorias: [] });
  gerenteUsuario = signal('');
  gerentePassword = signal('');
  gerenteValidado = signal(false);
  faCheck = faCheck;
  faXmark = faXmark;
  faTrash = faTrash;
  availableCategories = ['Distribuidora', 'Mayorista', 'Minorista', 'Insumos'];
  customCategory = '';

  puedeGuardar = computed(() => {
    const prov = this.nuevoProveedor();
    return prov.nombre.trim().length > 2 && prov.contacto.trim().length > 2 && (prov.categorias ?? []).length > 0 && (prov.calle?.trim().length ?? 0) > 0 && (prov.ciudad?.trim().length ?? 0) > 0 && this.gerenteValidado();
  });

  validarCredencialesGerente(): void {
    const user = this.gerenteUsuario().trim();
    const pass = this.gerentePassword();
    if (user.length > 2 && pass.length >= 6) {
      this.gerenteValidado.set(true);
    } else {
      this.gerenteValidado.set(false);
    }
  }

  toggleCategoria(cat: string): void {
    const actuales = [...(this.nuevoProveedor().categorias ?? [])];
    const idx = actuales.indexOf(cat);
    if (idx >= 0) {
      actuales.splice(idx, 1);
    } else {
      actuales.push(cat);
    }
    this.nuevoProveedor.update(v => ({ ...v, categorias: actuales }));
  }

  agregarCategoriaPersonalizada(): void {
    const text = (this.customCategory || '').trim();
    if (!text) return;
    const actuales = [...(this.nuevoProveedor().categorias ?? [])];
    if (!actuales.includes(text)) {
      actuales.push(text);
      this.nuevoProveedor.update(v => ({ ...v, categorias: actuales }));
    }
    if (!this.availableCategories.includes(text)) {
      this.availableCategories = [...this.availableCategories, text];
    }
    this.customCategory = '';
  }

  removerCategoria(cat: string): void {
    const actuales = (this.nuevoProveedor().categorias ?? []).filter(c => c !== cat);
    this.nuevoProveedor.update(v => ({ ...v, categorias: actuales }));
  }

  cancelar(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  guardarProveedor(): void {
    const proveedor = this.nuevoProveedor();
    if (!this.puedeGuardar()) return;

    this.proveedorService.crearProveedor(proveedor).subscribe(() => {
      // al guardar volvemos a la lista y notificamos con state
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], { state: { created: true, message: 'Proveedor creado correctamente' } });
    });
  }
}
