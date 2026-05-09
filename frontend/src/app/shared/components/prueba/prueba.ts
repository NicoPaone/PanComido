import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-prueba',
  imports: [],
  templateUrl: './prueba.html',
  styleUrl: './prueba.css',
})
export class Prueba {
  http = inject(HttpClient);
  endpoint = environment.apiUrl + '/weatherforecast'; 

  constructor() {
    console.log('Endpoint configurado:', this.endpoint);
    this.testearConexion();
  }
  NgOnInit() {
    console.log('Endpoint configurado en OnInit:', this.endpoint);
    this.testearConexion();
  }
  testearConexion() {
    this.http.get(this.endpoint).subscribe({
      next: (response) => {
        console.log('Respuesta del backend:', response);
      },
      error: (error) => {
        console.log(this.endpoint);
        console.error('Error al conectar con el backend:', error);
      }
    });
  }}
