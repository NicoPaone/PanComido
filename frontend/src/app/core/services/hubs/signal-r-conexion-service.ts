import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class SignalRConnectionService {
  // Ahora es público para que los servicios de dominio puedan suscribirse a eventos
  public hub: signalR.HubConnection; 
  private baseUrl = environment.apiUrl;
  
  // Guardamos la promesa de conexión para evitar condiciones de carrera
  private promesaInicio: Promise<void> | null = null;

  constructor() {
    // 1. Endpoint harcodeado: Ya no recibimos parámetros porque solo hay 1 Hub
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/hubs/pancomido`, {
         withCredentials: true,
      })
      .withAutomaticReconnect()
      .build();
  }

  public iniciar(): Promise<void> {
    // Si ya estamos conectados, salimos rápido
    if (this.hub.state === signalR.HubConnectionState.Connected) {
      return Promise.resolve();
    }

    // 2. Patrón Singleton de Promesa: Si nadie está conectando, iniciamos el proceso
    if (!this.promesaInicio) {
      this.promesaInicio = this.hub.start()
        .then(() => {
          console.log(" Conexión ÚNICA establecida con el servidor");
        })
        .catch(error => {
          console.error("Error al iniciar el hub: ", error);
          this.promesaInicio = null; // Reseteamos para que se pueda reintentar
          throw error; // Propagamos el error al servicio de dominio
        });
    }
    
    // Todos los servicios que llamen a iniciar() al mismo tiempo esperarán la misma conexión
    return this.promesaInicio;
  }
  
  public detener(): void {
    if (this.hub.state === signalR.HubConnectionState.Connected) {
      this.hub.stop();
      this.promesaInicio = null; // Limpiamos la promesa en caché
      console.log(` SignalR desconectado`);
    }
  }
}