import { Component, ElementRef, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { BrowserMultiFormatReader } from '@zxing/browser';
import { IScannerControls } from '@zxing/browser/esm/common/IScannerControls';
import { HeaderEscanearMesa } from '../components/header-escanear-mesa/header-escanear-mesa';

@Component({
  selector: 'app-escanear-mesa',
  imports: [HeaderEscanearMesa],
  templateUrl: './escanear-mesa.html',
  styleUrls: ['./escanear-mesa.css']
})
export class ScanQr implements AfterViewInit, OnDestroy {

  @ViewChild('video')
  video!: ElementRef<HTMLVideoElement>;

  scanner = new BrowserMultiFormatReader();
  private controls: IScannerControls | null = null;
  private scanResult = false;

  constructor(private router: Router) {}

  async ngAfterViewInit() {
    try {
      const devices = await BrowserMultiFormatReader.listVideoInputDevices();
      if (devices.length === 0) {
        console.error('No se encontró cámara');
        return;
      }

      this.controls = await this.scanner.decodeFromVideoDevice(
        devices[0].deviceId,
        this.video.nativeElement,
        (result) => {
          if (result && !this.scanResult) {
            this.scanResult = true;
            const texto = result.getText();
            const mesaId = parseInt(texto, 10);
            if (!isNaN(mesaId)) {
              this.router.navigate(['/comensal/nro-de-mesa'], {
                state: { mesaId }
              });
            }
          }
        }
      );
    } catch (e) {
      console.error('Error al acceder a la cámara', e);
    }
  }

  ngOnDestroy() {
    this.controls?.stop();
  }

  cerrarScanner() {
    this.controls?.stop();
    window.history.back();
  }
}
