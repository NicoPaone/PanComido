import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header-nro-de-mesa',
  templateUrl: './header-nro-de-mesa.html',
  styleUrls: ['./header-nro-de-mesa.css']
})
export class HeaderNroDeMesa {

  @Input() logoUrl: string = 'assets/images/logo/logo_el_ferroviario.png';

  constructor(private router: Router) {}

  volverAtras() {
    this.router.navigate(['/comensal/escanear-mesa']);
  }
}