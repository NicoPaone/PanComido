import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-llamar-al-mozo',
  standalone: true,
  templateUrl: './llamar-al-mozo.html',
  styleUrls: ['./llamar-al-mozo.css']
})
export class LlamarAlMozo {

  @Input() configuracion!: any;

  llamarMozo() {
    console.log('Llamando al mozo...');
  }
}