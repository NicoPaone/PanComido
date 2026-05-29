import { Component, computed, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark, faTrash } from '@fortawesome/free-solid-svg-icons';
import { toSignal } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Router } from '@angular/router';
import { NuevoProveedor } from '../../../../core/models/proveedor';
import { NuevoProveedorStateService } from '../services/nuevo-proveedor.state';

@Component({
  selector: 'app-nuevo-proveedor',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, FontAwesomeModule, Boton],
  templateUrl: './nuevo-proveedor.html',
  styleUrls: ['./nuevo-proveedor.css']
})
export class NuevoProveedorComponent {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly state = inject(NuevoProveedorStateService);

  proveedorForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3)]],
    contacto: ['', [Validators.required, Validators.minLength(3)]],
    telefono: ['', [Validators.pattern(/^\+?[0-9\s-]{7,15}$/)]],
    email: ['', [Validators.email]],
    calle: ['', [Validators.required]],
    numero: [''],
    ciudad: ['', [Validators.required]],
    customCategory: ['']
  });

  gerenteForm = this.fb.group({
    usuario: ['', [Validators.required, Validators.minLength(3)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]]
  });

  categorias = this.state.categorias;
  gerenteValidado = this.state.gerenteValidado;
  mensajeErrorGerente = this.state.mensajeErrorGerente;
  cargandoGerente = this.state.cargandoGerente;

  faCheck = faCheck;
  faXmark = faXmark;
  faTrash = faTrash;

  get availableCategories(): string[] {
    return this.state.availableCategories();
  }

  // Convertimos el observable del estado del formulario a Signal
  private readonly formStatus = toSignal(this.proveedorForm.statusChanges, {
    initialValue: this.proveedorForm.status
  });

  puedeGuardar = computed(() => {
    return this.formStatus() === 'VALID' && this.categorias().length > 0 && this.gerenteValidado();
  });

  validarCredencialesGerente(): void {
    const user = this.gerenteForm.get('usuario')?.value?.trim();
    const pass = this.gerenteForm.get('contrasena')?.value;
    if (!user || !pass || this.gerenteForm.invalid) {
      this.mensajeErrorGerente.set('Por favor, ingresa credenciales válidas (Usuario >= 3 car., Contraseña >= 6 car.).');
      return;
    }

    this.state.validarCredencialesGerente(user, pass, () => {
      this.gerenteForm.disable();
    });
  }

  toggleCategoria(cat: string): void {
    this.state.toggleCategoria(cat);
  }

  agregarCategoriaPersonalizada(): void {
    const text = this.proveedorForm.get('customCategory')?.value || '';
    this.state.agregarCategoriaPersonalizada(text, () => {
      this.proveedorForm.get('customCategory')?.reset();
    });
  }

  removerCategoria(cat: string): void {
    this.state.removerCategoria(cat);
  }

  cancelar(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  guardarProveedor(): void {
    if (!this.puedeGuardar()) return;
    
    const formVal = this.proveedorForm.value;
    const proveedor: NuevoProveedor = {
      nombre: formVal.nombre!,
      contacto: formVal.contacto!,
      telefono: formVal.telefono ?? '',
      email: formVal.email ?? '',
      calle: formVal.calle ?? '',
      numero: formVal.numero ?? '',
      ciudad: formVal.ciudad ?? '',
      categorias: this.categorias()
    };

    this.state.guardarProveedor(proveedor, () => {
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], { 
        state: { created: true, message: 'Proveedor creado correctamente' } 
      });
    });
  }
}
