import { Component, EventEmitter, Input, Output } from '@angular/core';


@Component({
  selector: 'app-header-escanear-mesa',
  templateUrl: './header-escanear-mesa.html',
  styleUrls: ['./header-escanear-mesa.css']
})
export class HeaderEscanearMesa {

  @Input() title: string = 'Mesa'; // o "Fila virtual"
    @Input() logoUrl: string = 'assets/images/logo/logo_el_ferroviario.png';

  @Output() close = new EventEmitter<void>();

  onClose() {
    this.close.emit();
  }
}