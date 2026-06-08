import { Injectable, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ProveedorApiService } from '../../services/proveedor.api';
import { ProveedorNuevo } from '../../../../core/models/domain/proveedor';

@Injectable({ providedIn: 'root' })
export class NuevoProveedorState {
  private api = inject(ProveedorApiService);
  private destroyRef = inject(DestroyRef);

  // Estado mutable expuesto como writeable signals
  categorias = signal<string[]>([]);
  gerenteValidado = signal(false);
  mensajeErrorGerente = signal<string | null>(null);
  cargandoGerente = signal(false);
  availableCategories = signal<string[]>(['Distribuidora', 'Mayorista', 'Minorista', 'Insumos']);

  validarCredencialesGerente(user: string, pass: string, onValid: () => void): void {
    if (!user || !pass) {
      this.mensajeErrorGerente.set('Por favor, ingresa credenciales válidas (Usuario >= 3 car., Contraseña >= 6 car.).');
      return;
    }

    this.cargandoGerente.set(true);
    this.mensajeErrorGerente.set(null);

    this.api.validateManagerCredentials(user, pass)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (esValido) => {
          this.cargandoGerente.set(false);
          this.gerenteValidado.set(esValido);
          if (!esValido) {
            this.mensajeErrorGerente.set('Usuario o contraseña de gerente incorrectos. (Prueba con "gerente" / "123456")');
          } else {
            onValid();
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

  agregarCategoriaPersonalizada(text: string, onReset: () => void): void {
    const cleanText = text.trim();
    if (!cleanText) return;
    const actuales = [...this.categorias()];
    if (!actuales.includes(cleanText)) {
      actuales.push(cleanText);
      this.categorias.set(actuales);
    }
    const avail = [...this.availableCategories()];
    if (!avail.includes(cleanText)) {
      this.availableCategories.set([...avail, cleanText]);
    }
    onReset();
  }

  removerCategoria(cat: string): void {
    const actuales = this.categorias().filter(c => c !== cat);
    this.categorias.set(actuales);
  }

  guardarProveedor(proveedor: ProveedorNuevo, onSuccess: () => void): void {
    this.api.crearProveedor(proveedor)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          onSuccess();
        }
      });
  }
}
