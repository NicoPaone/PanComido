import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';

@Component({
  selector: 'app-dropdown',
  imports: [],
  templateUrl: './dropdown.html',
  styleUrl: './dropdown.css',
})
export class Dropdown {

  estadoAbierto = signal<boolean>(false);
  private elementRef = inject(ElementRef);

  toggle(){
    this.estadoAbierto.update(isOpen => !isOpen);
  }

  cerrar(){
    this.estadoAbierto.set(false);
  }
  @HostListener('document:click', ['$event'])
  clickFuera(event:Event){
    if(!this.elementRef.nativeElement.contains(event.target)){
      this.cerrar();
    }
  }
}
