import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal, DestroyRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark, faTrash } from '@fortawesome/free-solid-svg-icons';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Router } from '@angular/router';
import { ProveedorService } from '../../../../core/services/proveedor.service';
import { AuthService } from '../../../../core/services/auth.service';
import { NuevoProveedor } from '../../../../core/models/proveedor';

@Component({
  selector: 'app-nuevo-proveedor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, FontAwesomeModule, Boton],
  templateUrl: './nuevo-proveedor.html',
  styleUrls: ['./nuevo-proveedor.css']
})
export class NuevoProveedorComponent {
  private readonly proveedorService = inject(ProveedorService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

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

  categorias = signal<string[]>([]);
  gerenteValidado = signal(false);
  mensajeErrorGerente = signal<string | null>(null);
  cargandoGerente = signal(false);

  faCheck = faCheck;
  faXmark = faXmark;
  faTrash = faTrash;
  availableCategories = ['Distribuidora', 'Mayorista', 'Minorista', 'Insumos'];

  puedeGuardar = computed(() => {
    return this.proveedorForm.valid && this.categorias().length > 0 && this.gerenteValidado();
  });

  validarCredencialesGerente(): void {
    const user = this.gerenteForm.get('usuario')?.value?.trim();
    const pass = this.gerenteForm.get('contrasena')?.value;
    if (!user || !pass || this.gerenteForm.invalid) {
      this.mensajeErrorGerente.set('Por favor, ingresa credenciales válidas (Usuario >= 3 car., Contraseña >= 6 car.).');
      return;
    }

    this.cargandoGerente.set(true);
    this.mensajeErrorGerente.set(null);

    this.authService.validateManagerCredentials(user, pass)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (esValido) => {
          this.cargandoGerente.set(false);
          this.gerenteValidado.set(esValido);
          if (!esValido) {
            this.mensajeErrorGerente.set('Usuario o contraseña de gerente incorrectos. (Prueba con "gerente" / "123456")');
          } else {
            this.gerenteForm.disable();
          }
        },
        error: () => {
          this.cargandoGerente.set(false);
          this.mensajeErrorGerente.set('Error de red al validar credenciales.');
        }
      });
  }

  toggleCategoria(cat: string): void {
    const actuales = [...this.categorias()];
    const idx = actuales.indexOf(cat);
    if (idx >= 0) {
      actuales.splice(idx, 1);
    } else {
      actuales.push(cat);
    }
    this.categorias.set(actuales);
  }

  agregarCategoriaPersonalizada(): void {
    const text = (this.proveedorForm.get('customCategory')?.value || '').trim();
    if (!text) return;
    const actuales = [...this.categorias()];
    if (!actuales.includes(text)) {
      actuales.push(text);
      this.categorias.set(actuales);
    }
    if (!this.availableCategories.includes(text)) {
      this.availableCategories = [...this.availableCategories, text];
    }
    this.proveedorForm.get('customCategory')?.reset();
  }

  removerCategoria(cat: string): void {
    const actuales = this.categorias().filter(c => c !== cat);
    this.categorias.set(actuales);
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

    // NOTE: El endpoint del back para registrar proveedores debe conectarse aquí
    this.proveedorService.crearProveedor(proveedor)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], { 
            state: { created: true, message: 'Proveedor creado correctamente' } 
          });
        },
        error: () => {
          // NOTE: El manejo de errores de comunicación debe integrarse aquí
        }
      });
  }
}
