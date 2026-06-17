import { Injectable } from '@angular/core';

export interface SesionComensal {
  restauranteId: number;
  mesaId: number;
  comandaId?: number;
  nombreComensal?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SesionComensalService {

  guardar(sesion: SesionComensal): void {
    sessionStorage.setItem(
      'sesionComensal',
      JSON.stringify(sesion)
    );
  }

  obtener(): SesionComensal | null {
    const data = sessionStorage.getItem('sesionComensal');

    return data ? JSON.parse(data) : null;
  }

  limpiar(): void {
    sessionStorage.removeItem('sesionComensal');
  }
}