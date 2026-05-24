import { Component, ElementRef, input, output, signal, viewChild } from '@angular/core';

@Component({
  selector: 'app-modal',
  imports: [],
  templateUrl: './modal.html',
  styleUrl: './modal.css',
})
export class Modal {

  ngOnInit(){
    console.log("Modal inicializado");
  }
  
  titulo = input<string>();

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
    const dialog = this.modalRef().nativeElement;
    const rect = dialog.getBoundingClientRect();
    
    // Si el clic ocurre fuera de las coordenadas de la caja blanca, cerramos
    const isInDialog = (rect.top <= event.clientY && event.clientY <= rect.top + rect.height &&
      rect.left <= event.clientX && event.clientX <= rect.left + rect.width);
      
    if (!isInDialog) {
      this.cerrar();
    }
  }
}
