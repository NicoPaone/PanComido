import { Component, ElementRef, input, output, signal, viewChild } from '@angular/core';

@Component({
  selector: 'app-modal',
  imports: [],
  templateUrl: './modal.html',
  styleUrl: './modal.css',
})
export class Modal {

  
  
  titulo = input<string>();
  maxWidth = input<number>(500);

  cerrado = output<void>();
  modalRef= viewChild.required<ElementRef<HTMLDialogElement>>('modalRef');

  abierto = signal(false);
  abrir() {
    this.abierto.set(true);
    this.modalRef().nativeElement.showModal();
  }

  cerrar() {
    this.modalRef().nativeElement.close();
  }

  onClose() {
    this.abierto.set(false); // Sincroniza el estado si cierran con tecla ESC
    this.cerrado.emit();
  }

  // Detecta si el clic fue en el fondo oscuro
  onBackdropClick(event: MouseEvent) {
     // Si el clic cayó exactamente en el fondo (el dialog host) y no en un div hijo
    if (event.target === this.modalRef().nativeElement) {
      this.cerrar();
    }
  
  }
}
