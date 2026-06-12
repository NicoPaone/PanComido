import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { DatosLocal } from '../../../../../core/models/domain/datos-local';
import { FamiliaTipografica } from '../../../../../core/models/domain/familia-tipografica';

@Component({
  selector: 'app-comensal-preview',
  imports: [],
  templateUrl: './comensal-preview.html',
  styleUrl: './comensal-preview.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ComensalPreviewComponent {
  readonly datosLocal = input.required<DatosLocal>();
  readonly familiasTipograficas = input<FamiliaTipografica[]>([]);

  readonly familiaActual = computed(() => {
    const id = this.datosLocal().familiaTipograficaId;
    return this.familiasTipograficas().find(f => f.id === id);
  });

  readonly fontTitulo = computed(() => {
    const familia = this.familiaActual();
    return familia ? `${familia.tipografiaTitulo}, sans-serif` : 'system-ui, sans-serif';
  });

  readonly fontCuerpo = computed(() => {
    const familia = this.familiaActual();
    return familia ? `${familia.tipografiaCuerpo}, sans-serif` : 'system-ui, sans-serif';
  });

  readonly colorPrimario = computed(() => this.datosLocal().colorPrincipal || '#1a4a2e');
  readonly colorSecundario = computed(() => this.datosLocal().colorSecundario || '#f08f1a');
  
  private getContrastColor(hex: string): string {
    if (!hex) return '#000000';
    hex = hex.replace('#', '');
    if (hex.length === 3) {
      hex = hex.split('').map(c => c + c).join('');
    }
    if (hex.length !== 6) return '#000000';
    const r = parseInt(hex.substring(0, 2), 16);
    const g = parseInt(hex.substring(2, 2), 16);
    const b = parseInt(hex.substring(4, 2), 16);
    const yiq = ((r * 299) + (g * 587) + (b * 114)) / 1000;
    return (yiq >= 128) ? '#000000' : '#ffffff';
  }
  
  readonly nombreLocal = computed(() => this.datosLocal().nombre || 'Nombre del local');
  readonly logo = computed(() => {
    const img = this.datosLocal().imagen;
    return img ? img : 'assets/images/default-logo.png';
  });

  // Mocks
  readonly platosMock = [
    {
      nombre: 'Pollo a la crema',
      esPlatoDelDia: true,
      tiempo: "20'",
      precio: '$ 18.300',
      img: 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&q=80&w=200&h=200'
    },
    {
      nombre: 'Pastel de papa',
      esPlatoDelDia: false,
      tiempo: "20'",
      precio: '$ 14.800',
      img: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?auto=format&fit=crop&q=80&w=200&h=200'
    },
    {
      nombre: 'Milanesa napolitana',
      esPlatoDelDia: false,
      tiempo: "25'",
      precio: '$ 16.200',
      img: 'https://images.unsplash.com/photo-1599921841143-819065a55cc6?auto=format&fit=crop&q=80&w=200&h=200'
    }
  ];
}
