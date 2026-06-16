import { ChangeDetectionStrategy, Component, effect, input, output, signal } from '@angular/core';
import { DatosLocal, DatosLocalEditables } from '../../../../../core/models/domain/datos-local';
import { FormsModule } from '@angular/forms';
import { FamiliaTipografica } from '../../../../../core/models/domain/familia-tipografica';

@Component({
  selector: 'app-datos-local-form',
  imports: [FormsModule],
  templateUrl: './datos-local-form.html',
  styleUrl: './datos-local-form.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DatosLocalForm {
  readonly datosLocal = input.required<DatosLocal>();
  readonly datosLocalChange = output<Partial<DatosLocalEditables>>();
  readonly familiasTipograficas = input<FamiliaTipografica[]>([]); 
  readonly archivoCambiar = output<File | null>();

  previewUrl = signal<string | null>(null);

  constructor() {
    effect(() => {
      const imagen = this.datosLocal().imagen;
      if (imagen && !imagen.startsWith('data:')) {
        this.previewUrl.set(imagen);
      } else if (imagen && imagen.startsWith('data:')) {
        this.previewUrl.set(imagen);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) return;

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl.set(reader.result as string);

      this.datosLocalChange.emit({imagen: reader.result as string});
    };
    reader.readAsDataURL(file);
    this.archivoCambiar.emit(file);

    input.value = '';
  }
  onCambiarFuente(familiaId: number):void{
    this.datosLocalChange.emit({familiaTipograficaId:familiaId})
  }

  removerImagen(): void {
    this.previewUrl.set(null);
    this.datosLocalChange.emit({ imagen: null });
    this.archivoCambiar.emit(null)
  }

  emitir(campo: keyof DatosLocalEditables, valor: string): void {
    this.datosLocalChange.emit({ [campo]: valor === '' ? null : valor });
  }
}