import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header-cantidad-personas',
  templateUrl: './header-cantidad-personas.html',
  styleUrls: ['./header-cantidad-personas.css']
})
export class HeaderCantidadPersonas {

  @Input() logoUrl: string = 'assets/images/logo/logo_el_ferroviario.png';

  constructor(private router: Router) {}

  volverAtras() {
    this.router.navigate(['/comensal/nro-de-mesa']);
  }
}