import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LlamarAlMozo } from '../llamar-al-mozo/llamar-al-mozo';

@Component({
  selector: 'app-comensal-footer-cart',
  standalone: true,
  imports: [CommonModule, LlamarAlMozo],
  templateUrl: './comensal-footer-cart.html',
  styleUrls: ['./comensal-footer-cart.css']
})
export class ComensalFooterCart {
  cantidadItems = input.required<number>();
  total = input.required<number>();
  configuracion = input.required<any>();
  
  verPedido = output<void>();

  abrirLlamarMozo(): void {
    const modalComponent = document.querySelector('app-llamar-al-mozo');
    if (modalComponent) {
      (modalComponent as any).abrirModal?.();
    }
  }
}