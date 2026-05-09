import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';

@Component({
  selector: 'app-inicio',
  imports: [],
  templateUrl: './inicio.html',
  styleUrl: './inicio.css',
})


export class Inicio {
  http = inject(HttpClient);
  endpoint = 'https://localhost:7204/weatherforecast/';

  constructor() {
    this.testearConexion();
  }
  testearConexion() {
    this.http.get(this.endpoint).subscribe({
      next: (response) => {
        console.log('Respuesta del backend:', response);
      },
      error: (error) => {
        console.error('Error al conectar con el backend:', error);
      }
    });
  }
}
