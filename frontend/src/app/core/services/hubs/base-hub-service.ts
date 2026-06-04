import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class SignalRConexionService {
  
  
  public hub!: signalR.HubConnection;
  private baseUrl = environment.apiUrl;

  private promesaInicio:Promise<void>|null=null;
  constructor(){
    this.hub = new signalR.HubConnectionBuilder()
    .withUrl(`${this.baseUrl}/hubs/pancomido`,{
      withCredentials:true,
    }).withAutomaticReconnect()
    .build();
  }
  public iniciar():Promise<void> {
    if(this.hub.state === signalR.HubConnectionState.Connected){
      return Promise.resolve();
    }
    // Patrón Singleton de Promesa: Si nadie está conectando, iniciamos el proceso
    if (!this.promesaInicio) {
      this.promesaInicio = this.hub.start()
        .then(() => {
          
        })
        .catch(error => {
          
          this.promesaInicio = null; // Reseteamos para que se pueda reintentar
          throw error; // Propagamos el error al servicio de dominio
        });
    }
    return this.promesaInicio;
  }
  public detener():void{
    if(this.hub.state === signalR.HubConnectionState.Connected){
      this.hub.stop();
      this.promesaInicio =null;
      
    }
  }

 
}
