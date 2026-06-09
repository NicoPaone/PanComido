import { Component, computed, inject , ChangeDetectionStrategy, OnInit} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCheck, faXmark, faTrash } from '@fortawesome/free-solid-svg-icons';
import { toSignal } from '@angular/core/rxjs-interop';
import { Boton } from '../../../../shared/ui/botones/boton/boton';
import { Router } from '@angular/router';
import { ProveedorNuevo } from '../../../../core/models/domain/proveedor';
import { NuevoProveedorState } from '../services/nuevo-proveedor.state';
import { CategoriaInsumo } from '../../../../core/models/domain/categoria-insumo';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-nuevo-proveedor',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, FontAwesomeModule, Boton],
  templateUrl: './nuevo-proveedor.html',
  styleUrls: ['./nuevo-proveedor.css']
})
export class NuevoProveedorComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly state = inject(NuevoProveedorState);

  proveedorForm = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(3)]],
    telefono: ['', [Validators.pattern(/^\+?[0-9\s-]{7,15}$/)]],
  });

  gerenteForm = this.fb.group({
    usuario: ['', [Validators.required, Validators.minLength(3)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]]
  });

  categorias = this.state.categorias;
  categoriaIds = this.state.categoriaIds;
  categoriasDisponibles = this.state.categoriasDisponibles;
  errorCategorias = this.state.errorCategorias;
  gerenteValidado = this.state.gerenteValidado;
  mensajeErrorGerente = this.state.mensajeErrorGerente;
  cargandoGerente = this.state.cargandoGerente;

  faCheck = faCheck;
  faXmark = faXmark;
  faTrash = faTrash;

  ngOnInit(): void {
    this.state.cargarCategorias();
  }

  // Convertimos el observable del estado del formulario a Signal
  private readonly formStatus = toSignal(this.proveedorForm.statusChanges, {
    initialValue: this.proveedorForm.status
  });

  puedeGuardar = computed(() => {
    return this.formStatus() === 'VALID' && this.categoriaIds().length > 0 && this.gerenteValidado();
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

  toggleCategoria(categoria: CategoriaInsumo): void {
    this.state.toggleCategoria(categoria);
  }

  categoriaSeleccionada(id: number): boolean {
    return this.categoriaIds().includes(id);
  }

  removerCategoria(id: number): void {
    this.state.removerCategoria(id);
  }

  cancelar(): void {
    this.router.navigate(['/staff', 'gerente', 'ver-proveedores']);
  }

  guardarProveedor(): void {
    if (!this.puedeGuardar()) return;
    
    const formVal = this.proveedorForm.value;
    const proveedor: ProveedorNuevo = {
      nombre: formVal.nombre!,
      numeroTelefonoWsp: formVal.telefono ?? '',
      categoriaIds: this.categoriaIds()
    };

    this.state.guardarProveedor(proveedor, () => {
      this.router.navigate(['/staff', 'gerente', 'ver-proveedores'], { 
        state: { created: true, message: 'Proveedor creado correctamente' } 
      });
    });
  }
}
