import { Injectable, signal } from '@angular/core';

export interface ComandoVoz {
  mesaNumero: number;
  accion: 'aceptar' | 'finalizar';
  nuevoEstadoId: number
  timestamp: number;
}


@Injectable({
  providedIn: 'root',
})
export class ComandoVozService {
  private recognition: any;

  enEscucha = signal<boolean>(false);
  comandoDetectado = signal<ComandoVoz | null>(null);
  error = signal<string | null>(null);

  constructor() {
    this.iniciarEscuchaApi();
  }

  private iniciarEscuchaApi() {
    const SpeechRec = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition;

    if (!SpeechRec) {
      this.error.set('El navegador no soporta la API de reconocimiento de voz.');
      return;
    }
    this.recognition = new SpeechRec();
    this.recognition.lang = 'es-AR';
    this.recognition.continuous = true;
    this.recognition.interimResults = false;

    this.recognition.onresult = (event: any) => {
      const ultimoIndice = event.results.length - 1;
      const frase = event.results[ultimoIndice][0].transcript.toLowerCase().trim();
      this.procesarFrase(frase);
    }
    this.recognition.onerror = (event: any) => {
      console.error('Error de voz:', event.error);
      if (event.error !== 'no-speech') {
        this.error.set(`Error de micrófono: ${event.error}`);
        this.stop();
      }
    };
    this.recognition.onend = () => {
      // El navegador corta el mic si hay mucho silencio. 
      // Si el cocinero no lo apagó manualmente, lo volvemos a encender automáticamente.
      if (this.enEscucha()) {
        this.recognition.start();
      }
    };

  }
  private procesarFrase(frase: string) {
    console.log('Escuchando: ', frase);

    const matchAceptar = frase.match(/mesa (\d+) (aceptar|aceptada)/);
    const matchFinalizar = frase.match(/mesa (\d+) (finalizar|finalizada)/);

    if (matchFinalizar) {
      this.comandoDetectado.set({
        mesaNumero: Number(matchFinalizar[1]),
        accion: 'finalizar',
        nuevoEstadoId: 4, // EstadoComandaId.Finalizada
        timestamp: Date.now(),
      });
    } else if (matchAceptar) {
      this.comandoDetectado.set({
        mesaNumero: Number(matchAceptar[1]),
        accion: 'aceptar',
        nuevoEstadoId: 2, // EstadoComandaId.EnPreparacion
        timestamp: Date.now(),
      })
    }

  }
  toggleListening() {
    if (this.enEscucha()) {
      this.stop();
    } else {
      this.start();
    }
  }

  private start() {
    if (!this.recognition) return;
    this.enEscucha.set(true);
    this.error.set(null);
    this.recognition.start();
  }

  private stop() {
    if (!this.recognition) return;
    this.enEscucha.set(false);
    this.recognition.stop();
  }







}
