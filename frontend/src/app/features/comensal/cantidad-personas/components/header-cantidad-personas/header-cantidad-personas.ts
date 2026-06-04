import { Component, EventEmitter, Input, Output , ChangeDetectionStrategy} from '@angular/core';
import { Router } from '@angular/router';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
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