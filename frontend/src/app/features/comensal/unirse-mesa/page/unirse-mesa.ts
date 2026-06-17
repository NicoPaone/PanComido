import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ComandaState } from '../../services/comanda-state';

@Component({
  selector: 'app-unirse-mesa',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './unirse-mesa.html',
  styleUrls: ['./unirse-mesa.css']
})
export class UnirseMesa implements OnInit {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private comandaState = inject(ComandaState);

  comandaId!: number;
  datosMesa: any;
  nombre = signal('');

  ngOnInit(): void {
    this.comandaId = Number(this.route.snapshot.paramMap.get('comandaId'));

    this.cargarBienvenida();
  }

  cargarBienvenida() {
    this.comandaState.obtenerBienvenidaInvitado(this.comandaId)
      .subscribe({
        next: (res) => {
          this.datosMesa = res;
        },
        error: (err) => {
          console.error(err);
        }
      });
  }

  unirse() {
    if (!this.nombre()) return;

    // guardar sesión del invitado
    sessionStorage.setItem('sesionComensal', JSON.stringify({
      comandaId: this.comandaId,
      mesaId: this.datosMesa.mesaId,
      restauranteId: this.datosMesa.restauranteId,
      nombre: this.nombre()
    }));

    this.router.navigate(['/comensal/ver-carta']);
    
  }
}